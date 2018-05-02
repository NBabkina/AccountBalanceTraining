using System;
using AccountBalanceDomain;
using System.Threading.Tasks;
using AccountBalanceDomain.Commands;
using AccountBalanceDomain.Events;
using ReactiveDomain.Messaging;
using Xunit;
using Xunit.ScenarioReporting;

namespace AccountBalanceTest
{
    /// <summary>
    /// uses EventStoreScenarioRunner for testing
    /// </summary>

    [Collection("AggregateTest")]
    public sealed class WithdrawCashTests : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<BankAccount> _runner;

        public WithdrawCashTests(EventStoreFixture fixture)
        {
            _accountId = Guid.NewGuid();
            _runner = new EventStoreScenarioRunner<BankAccount>(
                _accountId,
                fixture,
                (repository, dispatcher) => new BankAccountCommandHandler(repository, dispatcher));
        }

        public void Dispose() => _runner.Dispose();

        // tests ------------------------------------------

        [Fact]
        public Task CannnotWithdrawCash_AccountDoesntExist()
        {
            WithdrawCashCommand cmd = new WithdrawCashCommand()
            {
                AccountId = _accountId,
                Amount = 1000
            };

            return _runner.Run(
                def => def.Given().When(cmd).Throws(new InvalidOperationException("Account does not exist")));
        }

        [Fact]
        public Task CanWithdrawCash()
        {
            var ev = new BankAccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            WithdrawCashCommand cmd = new WithdrawCashCommand()
            {
                AccountId = _accountId,
                Amount = 1000
            };

            var newEv = new AmountWithdrawnEvent(cmd)
            {
                AccountId = _accountId,
                Amount = 1000
            };

            return _runner.Run(
                def => def.Given(ev).When(cmd).Then(newEv));
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public Task CannotWithdrawCash_IllegalAmount(decimal am)
        {
            var ev = new BankAccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            WithdrawCashCommand cmd = new WithdrawCashCommand()
            {
                AccountId = _accountId,
                Amount = am
            };

            return _runner.Run(
                def => def.Given(ev).When(cmd).Throws(new InvalidOperationException("Amount to withdraw must be > 0")));
        }

    }
}

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
        readonly EventStoreScenarioRunner<Account> _runner;

        public WithdrawCashTests(EventStoreFixture fixture)
        {
            _accountId = Guid.NewGuid();
            _runner = new EventStoreScenarioRunner<Account>(
                _accountId,
                fixture,
                (repository, dispatcher) => new AccountCommandHandler(repository, dispatcher));
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

        [Theory]
        [InlineData(10)]
        [InlineData(1000)]
        public Task CanWithdrawCash(decimal am)
        {
            var ev1 = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            var ev2 = new OverdraftLimitIsSetEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                OverdraftLimit = 1000
            };

            WithdrawCashCommand cmd = new WithdrawCashCommand()
            {
                AccountId = _accountId,
                Amount = am
            };

            var newEv = new AmountWithdrawnEvent(cmd)
            {
                AccountId = _accountId,
                Amount = am
            };

            return _runner.Run(
                def => def.Given(ev1, ev2).When(cmd).Then(newEv));
        }

        [Fact]
        public Task CannotWithdrawCash_EverdraftLimitexceeded()
        {
            var ev1 = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            var ev2 = new OverdraftLimitIsSetEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                OverdraftLimit = 1000
            };

            var ev3 = new AmountDepositedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Amount = 100
            };

            WithdrawCashCommand cmd = new WithdrawCashCommand()
            {
                AccountId = _accountId,
                Amount = 2000
            };

            var newEv = new AccountBlockedEvent(cmd)
            {
                AccountId = _accountId,
            };

            return _runner.Run(
                def => def.Given(ev1, ev2, ev3).When(cmd).Then(newEv));
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public Task CannotWithdrawCash_IllegalAmount(decimal am)
        {
            var ev = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
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

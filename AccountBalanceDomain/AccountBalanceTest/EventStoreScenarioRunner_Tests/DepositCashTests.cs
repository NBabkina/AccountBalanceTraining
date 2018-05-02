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
    public sealed class DepositCashTests : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public DepositCashTests(EventStoreFixture fixture)
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
        public Task CannnotDepositCash_AccountDoesntExist()
        {
            DepositCashCommand cmd = new DepositCashCommand()
            {
                AccountId = _accountId,
                Amount = 1000
            };

            return _runner.Run(
                def => def.Given().When(cmd).Throws(new InvalidOperationException("Account does not exist")));
        }

        [Fact]
        public Task CanDepositCash()
        {
            var ev = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            DepositCashCommand cmd = new DepositCashCommand()
            {
                AccountId = _accountId,
                Amount = 1000
            };

            var newEv = new AmountDepositedEvent(cmd)
            {
                AccountId = _accountId,
                Amount = 1000
            };

            return _runner.Run(
                def => def.Given(ev).When(cmd).Then(newEv));
        }

        [Fact]
        public Task CanDepositCashAndUnblock()
        {
            var ev1 = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            var ev2 = new AccountBlockedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
            };

            DepositCashCommand cmd = new DepositCashCommand()
            {
                AccountId = _accountId,
                Amount = 1000
            };

            var newEv1 = new AmountDepositedEvent(cmd)
            {
                AccountId = _accountId,
                Amount = 1000
            };

            var newEv2 = new AccountUnblockedEvent(cmd)
            {
                AccountId = _accountId

            };

            return _runner.Run(
                def => def.Given(ev1, ev2).When(cmd).Then(newEv1, newEv2));
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public Task CannotDepositCash_IllegalAmount(decimal am)
        {
            var ev = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            DepositCashCommand cmd = new DepositCashCommand()
            {
                AccountId = _accountId,
                Amount = am
            };

            return _runner.Run(
                def => def.Given(ev).When(cmd).Throws(new InvalidOperationException("Amount to deposit must be > 0")));
        }

    }


    
}
using System;
using System.Threading.Tasks;
using AccountBalanceDomain;
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

    public sealed class SetupAccountDailyTests : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public SetupAccountDailyTests(EventStoreFixture fixture)
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
        public Task CannotSetupAccountDaily_AccountDoesntExist()
        {
            SetupAccountDailyCommand cmd = new SetupAccountDailyCommand()
            {
                AccountId = _accountId,
            };

            return _runner.Run(
                def => def.Given().When(cmd).Throws(new InvalidOperationException("Account does not exist")));
        }

        [Fact]
        public Task CanSetupAccountDaily()
        {
            var ev1 = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            var ev2 = new ChequeDepositedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Amount = 100
            };

            var ev3 = new ChequeDepositedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                Amount = 200
            };

            var cmd = new SetupAccountDailyCommand()
            {
                AccountId = _accountId,
            };

            var newEv = new ChequesClearedEvent(cmd)
            {
                AccountId = _accountId,
                Amount = 300
            };

            return _runner.Run(
                def => def.Given(ev1, ev2, ev3).When(cmd).Then(newEv));
        }


        [Fact]
        public Task CanSetupAccountDaily_NoPendingAmount()
        {
            var ev1 = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            var cmd = new SetupAccountDailyCommand()
            {
                AccountId = _accountId,
            };

            return _runner.Run(
                def => def.Given(ev1).When(cmd).Then());
        }

    }
}
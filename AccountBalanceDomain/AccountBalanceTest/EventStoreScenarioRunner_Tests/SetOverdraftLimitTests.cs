using System;
using AccountBalanceDomain;
using Xunit;
using System.Threading.Tasks;
using AccountBalanceDomain.Commands;
using AccountBalanceDomain.Events;
using ReactiveDomain.Messaging;
using Xunit.ScenarioReporting;

namespace AccountBalanceTest
{
    /// <summary>
    /// uses EventStoreScenarioRunner for testing
    /// </summary>

    [Collection("AggregateTest")]

    public sealed class SetOverdraftLimitTests : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public SetOverdraftLimitTests(EventStoreFixture fixture)
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
        public Task CannnotSetOverdraftLimit_AccountDoesntExist()
        {
            SetOverdraftLimitCommand cmd = new SetOverdraftLimitCommand()
            {
                AccountId = _accountId,
                OverdraftLimit = 1000
            };

            return _runner.Run(
                def => def.Given().When(cmd).Throws(new InvalidOperationException("Account does not exist")));
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1000)]
        [InlineData(1000000)]
        public Task CanSetOverdraftLimit(decimal ovLimit)
        {
            var ev = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            var cmd = new SetOverdraftLimitCommand()
            {
                AccountId = _accountId,
                OverdraftLimit = ovLimit
            };

            var newEv = new OverdraftLimitIsSetEvent(cmd)
            {
                AccountId = _accountId,
                OverdraftLimit = cmd.OverdraftLimit
            };

            return _runner.Run(
                def => def.Given(ev).When(cmd).Then(newEv));
        }
        
        
        [Fact]
        public Task CannnotSetOverdraftLimit_NegativeValue()
        {
            var ev = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            var cmd = new SetOverdraftLimitCommand()
            {
                AccountId = _accountId,
                OverdraftLimit = -1000
            };

            return _runner.Run(
                def => def.Given(ev).When(cmd).Throws(new InvalidOperationException("Overdraft limit must be >= 0")));
        }

    }
}
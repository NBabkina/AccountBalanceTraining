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
    public sealed class SetTransferLimitTests : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public SetTransferLimitTests(EventStoreFixture fixture)
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
        public Task CannnotSetTransferLimit_AccountDoesntExist()
        {
            SetTransferLimitCommand cmd = new SetTransferLimitCommand()
            {
                AccountId = _accountId,
                TransferLimit = 1000
            };

            return _runner.Run(
                def => def.Given().When(cmd).Throws(new InvalidOperationException("Account does not exist")));
        }


        [Theory]
        [InlineData(0)]
        [InlineData(1000)]
        [InlineData(1000000)]
        public Task CanSetTransferLimit(decimal trLimit)
        {
            var ev = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            var cmd = new SetTransferLimitCommand()
            {
                AccountId = _accountId,
                TransferLimit = trLimit
            };

            var newEv = new TransferLimitIsSetEvent(cmd)
            {
                AccountId = _accountId,
                TransferLimit = cmd.TransferLimit
            };

            return _runner.Run(
                def => def.Given(ev).When(cmd).Then(newEv));
        }


        [Fact]
        public Task CannnotSetTransferLimit_NegativeValue()
        {
            var ev = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            var cmd = new SetTransferLimitCommand()
            {
                AccountId = _accountId,
                TransferLimit = -1000
            };

            return _runner.Run(
                def => def.Given(ev).When(cmd).Throws(new InvalidOperationException("Transfer limit must be >= 0")));
        }


    }
}
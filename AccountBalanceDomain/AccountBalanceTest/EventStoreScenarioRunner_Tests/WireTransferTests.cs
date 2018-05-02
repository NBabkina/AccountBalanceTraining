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


    public sealed class WireTranferTests : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;

        public WireTranferTests(EventStoreFixture fixture)
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
        public Task CannnotWireTransfer_AccountDoesntExist()
        {
            WithdrawWireTransferCommand cmd = new WithdrawWireTransferCommand()
            {
                AccountId = _accountId,
                Amount = 1000
            };

            return _runner.Run(
                def => def.Given().When(cmd).Throws(new InvalidOperationException("Account does not exist")));
        }

        [Fact]
        public Task CannnotWithddrawWireTransfer_LimitIsNotSet()
        {
            var ev = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            WithdrawWireTransferCommand cmd = new WithdrawWireTransferCommand()
            {
                AccountId = _accountId,
                Amount = 1000
            };

            return _runner.Run(
                def => def.Given(ev).When(cmd).Throws(new InvalidOperationException("Wire transfer limit is set to 0")));
        }

        [Fact]
        public Task CannnotWithddrawWireTransfer_DailyLimitIsExceeded()
        {
            var ev1 = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            var ev2 = new TransferLimitIsSetEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                TransferLimit = 100
            };

            WithdrawWireTransferCommand cmd = new WithdrawWireTransferCommand()
            {
                AccountId = _accountId,
                Amount = 1000
            };

            var newEv = new AccountBlockedEvent(cmd)
            {
                AccountId = _accountId
            };

            return _runner.Run(
                def => def.Given(ev1, ev2).When(cmd).Then(newEv));

            //return _runner.Run(
            //def => def.Given(ev1, ev2).When(cmd).Throws(new InvalidOperationException("Unable to withdraw - daily limit is exceeded")));
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public Task CannnotWithddrawWireTransfer_IllegalAmount(decimal am)
        {
            var ev1 = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            var ev2 = new TransferLimitIsSetEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                TransferLimit = 100
            };

            WithdrawWireTransferCommand cmd = new WithdrawWireTransferCommand()
            {
                AccountId = _accountId,
                Amount = am
            };

            return _runner.Run(
                def => def.Given(ev1, ev2).When(cmd).Throws(new InvalidOperationException("Wire transfer amount must be > 0")));
        }

        [Fact]
        public Task CanWithddrawWireTransfer()
        {
            var ev1 = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            var ev2 = new TransferLimitIsSetEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                TransferLimit = 100
            };

            WithdrawWireTransferCommand cmd = new WithdrawWireTransferCommand()
            {
                AccountId = _accountId,
                Amount = 50
            };

            var newEv = new AmountWithdrawnEvent(cmd)
            {
                AccountId = _accountId,
                Amount = 50
            };

            return _runner.Run(
                def => def.Given(ev1, ev2).When(cmd).Then(newEv));
        }


    }
}
using System;
using AccountBalanceDomain;
using System.Threading.Tasks;
using AccountBalanceDomain.Commands;
using AccountBalanceDomain.Events;
using NodaTime;
using ReactiveDomain.Messaging;
using Xunit;
using Xunit.ScenarioReporting;

namespace AccountBalanceTest
{
    /// <summary>
    /// uses EventStoreScenarioRunner for testing
    /// </summary>

    [Collection("AggregateTest")]


    public sealed class DepositChequesTests : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<Account> _runner;
        private readonly IClock _clock;

        public DepositChequesTests(EventStoreFixture fixture)
        {

            _clock = SystemClock.Instance;
            
            _accountId = Guid.NewGuid();
            _runner = new EventStoreScenarioRunner<Account>(
                _accountId,
                fixture,
                (repository, dispatcher) =>
                {
                    var commandHandler = new AccountCommandHandler(repository, dispatcher)
                    {
                        Clock = _clock
                    };
                    return commandHandler;
                });
        }
    
        public void Dispose() => _runner.Dispose();

        // tests ------------------------------------------

        [Fact]
        public Task CannnotDepositCheque_AccountDoesntExist()
        {
            DepositChequeCommand cmd = new DepositChequeCommand()
            {
                AccountId = _accountId,
                Amount = 1000
            };

            return _runner.Run(
                def => def.Given().When(cmd).Throws(new InvalidOperationException("Account does not exist")));
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public Task CannotDepositCheque_IllegalAmount(decimal am)
        {
            var ev = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            DepositChequeCommand cmd = new DepositChequeCommand()
            {
                AccountId = _accountId,
                Amount = am
            };

            return _runner.Run(
                def => def.Given(ev).When(cmd).Throws(new InvalidOperationException("Amount to deposit a cheque must be > 0")));
        }

        [Fact]
        public Task CanDepositCheque()
        {
            var ev = new AccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            DepositChequeCommand cmd = new DepositChequeCommand()
            {
                AccountId = _accountId,
                Amount = 1000
            };

            var newEv = new ChequeDepositedEvent(cmd)
            {
                AccountId = _accountId,
                Amount = 1000
                //ChequeDepositDate = _clock.GetCurrentInstant()
            };

            return _runner.Run(
                def => def.Given(ev).When(cmd).Then(newEv));
        }

    }


}
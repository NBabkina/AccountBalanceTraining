using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountBalanceDomain;
using AccountBalanceDomain.Events;
using Xunit;
using ReactiveDomain.Messaging;
using Xunit.ScenarioReporting;

namespace AccountBalanceTest
{
    /// <summary>
    /// uses EventStoreScenarioRunner for testing
    /// </summary>

    [Collection("AggregateTest")]

    public sealed class CreateAccontTests : IDisposable
    {
        readonly Guid _accountId;
        readonly EventStoreScenarioRunner<BankAccount> _runner;

        public CreateAccontTests(EventStoreFixture fixture)
        {
            _accountId = Guid.NewGuid();
            _runner = new EventStoreScenarioRunner<BankAccount>(
                _accountId,
                fixture,
                (repository, dispatcher) => new BankAccountCommandHandler(repository, dispatcher));
        }

        [Fact]
        public async Task CanCreateAccount()
        {
            CreateBankAccountCommand cmd;
            cmd = new CreateBankAccountCommand(CorrelationId.NewId(), SourceId.NullSourceId())
            {
                AccountId = _accountId,
                AccountHolderName = "AccountHolder1"
            };

            _runner.Run(
                def => def.Given().When(cmd).Then(
                    new BankAccountCreatedEvent(cmd)
                    {
                        AccountId = cmd.AccountId,
                        AccountHolderName = cmd.AccountHolderName
                    }));


           await Task.CompletedTask;
        }

        [Fact]
        public async Task CannotCreateAccountIfExists()
        {
            BankAccountCreatedEvent ev = new BankAccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            CreateBankAccountCommand cmd = new CreateBankAccountCommand(CorrelationId.NewId(), SourceId.NullSourceId())
            {
                AccountId = _accountId,
                AccountHolderName = "BBB"
            };

            _runner.Run(
                def => def.Given(ev).When(cmd).Throws(new InvalidOperationException("Account already exists"), verifyMessage: false));

            await Task.CompletedTask;

        }

        [Fact]
        public async Task CannotCreateAcountWithEmptyHolder()
        {
            CreateBankAccountCommand cmd = new CreateBankAccountCommand(CorrelationId.NewId(), SourceId.NullSourceId())
            {
                AccountId = _accountId,
                AccountHolderName = string.Empty
            };

            _runner.Run(
                def => def.Given().When(cmd).Throws(new InvalidOperationException("Account holder name cannot be empty"), verifyMessage: false));

            await Task.CompletedTask;
        }






        public void Dispose()
        {
            _runner.Dispose();
        }


    }
}


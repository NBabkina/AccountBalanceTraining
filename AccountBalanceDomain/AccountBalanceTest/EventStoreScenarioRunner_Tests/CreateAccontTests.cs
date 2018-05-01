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

        public void Dispose() => _runner.Dispose();

        [Fact]
        public Task CanCreateAccount()
        {
            CreateBankAccountCommand cmd;
            cmd = new CreateBankAccountCommand()
            {
                AccountId = _accountId,
                AccountHolderName = "AccountHolder1"
            };

            return _runner.Run(
                def => def.Given().When(cmd).Then(
                    new BankAccountCreatedEvent(cmd)
                    {
                        AccountId = cmd.AccountId,
                        AccountHolderName = cmd.AccountHolderName
                    }));
        }

        [Fact]
        public Task CannotCreateAccountIfExists()
        {
            BankAccountCreatedEvent ev = new BankAccountCreatedEvent(CorrelatedMessage.NewRoot())
            {
                AccountId = _accountId,
                AccountHolderName = "AAA"
            };

            CreateBankAccountCommand cmd = new CreateBankAccountCommand()
            {
                AccountId = _accountId,
                AccountHolderName = "BBB"
            };

            return _runner.Run(
                def => def.Given(ev).When(cmd).Throws(
                    new InvalidOperationException("Account already exists"), verifyMessage: false));
        }

        [Fact]
        public Task CannotCreateAcountWithEmptyHolder()
        {
            CreateBankAccountCommand cmd = new CreateBankAccountCommand()
            {
                AccountId = _accountId,
                AccountHolderName = string.Empty
            };

            return _runner.Run(
                def => def.Given().When(cmd).Throws(
                    new InvalidOperationException("Account holder name cannot be empty"), verifyMessage: false));

        }
    }
}


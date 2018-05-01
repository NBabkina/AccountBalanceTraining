using System;
using Xunit;
using ReactiveDomain.Messaging;
using System.Threading.Tasks;
using AccountBalanceDomain;
using AccountBalanceDomain.Events;
using Xunit.ScenarioReporting;


namespace AccountBalanceTest
{
    /// <summary>
    /// uses DummyRepoScenarioRunner for testing
    /// </summary>
    public class AccountTests
    {
       static DummyRepoScenarioRunner<BankAccount> CreateRunner(Guid aggregateId)
        {
            return new DummyRepoScenarioRunner<BankAccount>(
                aggregateId,
                (repo, bus) =>
                {
                    var handler = new BankAccountCommandHandler(repo, bus);
                });
        }

        [Fact]
        public Task can_create_account()
        {
            var instId = Guid.NewGuid();
            var runner = CreateRunner(instId);

            var cmd = new CreateBankAccountCommand()
            //CorrelationId.NewId(), SourceId.NullSourceId()
            {
                AccountId = instId,
                AccountHolderName = "AccountHolder1"
            };

            return runner.Run(
                def => def.Given().When(cmd).Then(
                    new BankAccountCreatedEvent(cmd)
                    {
                        AccountId = instId,
                        AccountHolderName = "AccountHolder1"
                    }));
        }


        [Fact]
        public Task cannot_create_account_if_exists()
        {
            var instId = Guid.NewGuid();
            var runner = CreateRunner(instId);

            return runner.Run(
                def => def.Given(
                    new BankAccountCreatedEvent(CorrelatedMessage.NewRoot())
                    {
                        AccountId = instId,
                        AccountHolderName = "AAA"

                    }).When(
            //new CreateBankAccountCommand(CorrelationId.NewId(), SourceId.NullSourceId())
                    new CreateBankAccountCommand()
            {
                        AccountId = instId,
                        AccountHolderName = "BBB"
                    }).Throws(new InvalidOperationException("Account already exists"), verifyMessage: false));
        }


        [Fact]
        public Task cannot_create_account_with_empty_holder()
        {
            var instId = Guid.NewGuid();
            var runner = CreateRunner(instId);

            return runner.Run(def =>
                def.Given()
                    .When(new CreateBankAccountCommand()
                    {
                        AccountId = instId,
                        AccountHolderName = string.Empty
                    }).Throws(new InvalidOperationException("Account holder name cannot be empty"),verifyMessage: false));

        }


        //[Fact]
        //public Task can_set_ov_limit()
        //{
        //    var instId = Guid.NewGuid();
        //    var runner = CreateRunner(instId);

        //    return runner.Run(def =>
        //        def.Given()
        //            .When(new CreateBankAccountCommand(CorrelationId.NewId(), SourceId.NullSourceId())
        //            {
        //                AccountId = instId,
        //                AccountHolderName = string.Empty
        //            }).Throws(new InvalidOperationException("Account holder name cannot be empty"), verifyMessage: false));
        //}


        //public Task cannot_set_ov_limit_account_not_exists()
        //{
        //}

        //public Task cannot_set_ov_limit_less_than_zero
        //{

        //}




    }
}
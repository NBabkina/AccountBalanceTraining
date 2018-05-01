using System;
//using System.Collections.Generic;
//using AccountBalanceDomain.Commands;
using AccountBalanceDomain.Events;
using ReactiveDomain;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain
{
    public class BankAccount : EventDrivenStateMachine
    {
        //public Guid Id is inherited from base class
        //public string AccountHolderName { get; set; }

        private decimal _overdraft_limit;
        private decimal _transfer_limit;
        private decimal _balance;
        private decimal _pending_amount;


        private BankAccount()
        {
            // call Register() to store all types of operations we need to perform on each type of event
            Register<BankAccountCreatedEvent>(ev => 
            {
                Id = ev.AccountId;
                Console.WriteLine("BankAccountCreatedEvent raised for accID: " + ev.AccountId);
            });

            Register<OverdraftLimitIsSetEvent>(ev => { _overdraft_limit = ev.OverdraftLimit; });
            Register<TransferLimitIsSetEvent>(ev => { _transfer_limit = ev.TransferLimit; });

        }

        public static BankAccount Create(Guid id, string accountHolderName, CorrelatedMessage source)
        {
            if (string.IsNullOrWhiteSpace(accountHolderName))
                throw new InvalidOperationException("Account holder name cannot be empty");
            
            var acc = new BankAccount();

            acc.Raise(new BankAccountCreatedEvent(source) {AccountId = id, AccountHolderName = accountHolderName });

            return acc;
        }

        public void SetOverdraftLimit(decimal overdraftLimit, CorrelatedMessage source)
        {
            if (overdraftLimit < 0)
                throw new InvalidOperationException("Overdraft limit must be >= 0");

            this.Raise(new OverdraftLimitIsSetEvent(source) { AccountId = this.Id, OverdraftLimit = overdraftLimit});
        }

        public void SetTransferLimit(decimal transferLimit, CorrelatedMessage source)
        {
            if (transferLimit < 0)
                throw new InvalidOperationException("Transfer limit must be >= 0");

            this.Raise(new TransferLimitIsSetEvent(source) { AccountId = this.Id, TransferLimit = transferLimit });
        }

    }
}

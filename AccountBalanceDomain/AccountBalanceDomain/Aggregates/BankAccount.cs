using System;
using System.Collections.Generic;
using AccountBalanceDomain.Events;
using ReactiveDomain;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain
{
    public class BankAccount : EventDrivenStateMachine
    {
        //public Guid Id is inherited from base class
        public string AccountHolderName { get; set; }


        private BankAccount()
        {
            // call Register() to store all types of operations we need to perform on each type of event
            Register<BankAccountCreatedEvent>(OnBankAccountCreatedEvent);

        }

        public static BankAccount Create(Guid id, string accountHolderName, CorrelatedMessage source)
        {
            if (string.IsNullOrWhiteSpace(accountHolderName))
                throw new InvalidOperationException("Account holder name cannot be empty");
            
            var acc = new BankAccount();

            acc.Raise(new BankAccountCreatedEvent(source) {AccountId = id, AccountHolderName = accountHolderName });

            return acc;
        }

        private void OnBankAccountCreatedEvent(BankAccountCreatedEvent ev)
        {
            Id = ev.AccountId;
            //AccountHolderName = ev.AccountHolderName;
            Console.WriteLine("BankAccountCreatedEvent raised for accID: " + ev.AccountId);
        }


        //public void OnOver


    }
}

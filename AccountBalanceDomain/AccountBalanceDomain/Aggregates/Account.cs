using System;
using AccountBalanceDomain.Commands;
//using System.Collections.Generic;
//using AccountBalanceDomain.Commands;
using AccountBalanceDomain.Events;
using ReactiveDomain;
using ReactiveDomain.Messaging;
using NodaTime;

namespace AccountBalanceDomain
{
    public class Account : EventDrivenStateMachine
    {
        //public Guid Id is inherited from base class
        //public string AccountHolderName { get; set; }

        private decimal _overdraft_limit;
        private decimal _transfer_limit;

        private decimal _balance;
        private decimal _pending_amount;
        private decimal _used_transfer_limit;
        private bool _isBlocked = false;


        private Account()
        {
            // call Register() all operations we need to perform on each type of event
            // will be used to play back all events for the aggregate

            Register<AccountCreatedEvent>(ev => 
            {
                Id = ev.AccountId;
                Console.WriteLine("AccountCreatedEvent raised for accID: " + ev.AccountId);
            });

            Register<OverdraftLimitIsSetEvent>(ev => { _overdraft_limit = ev.OverdraftLimit; });
            Register<TransferLimitIsSetEvent>(ev => { _transfer_limit = ev.TransferLimit; });
            Register<AmountDepositedEvent>(ev => { _balance += ev.Amount; });
            Register<AmountWithdrawnEvent>(ev => { _balance -= ev.Amount; });
            Register<ChequeDepositedEvent>(ev => { _pending_amount += ev.Amount; });
            Register<ChequesClearedEvent>(ev => 
            {
                _balance += _pending_amount;
                _pending_amount = 0;
            });

        }

        public static Account Create(Guid id, string accountHolderName, CorrelatedMessage source)
        {
            if (string.IsNullOrWhiteSpace(accountHolderName))
                throw new InvalidOperationException("Account holder name cannot be empty");
            
            var acc = new Account();

            acc.Raise(new AccountCreatedEvent(source) {AccountId = id, AccountHolderName = accountHolderName });

            return acc;
        }

        public void SetOverdraftLimit(decimal overdraftLimit, CorrelatedMessage source)
        {
            if (overdraftLimit < 0)
                throw new InvalidOperationException("Overdraft limit must be >= 0");

            this.Raise(new OverdraftLimitIsSetEvent(source)
            {
                AccountId = this.Id,
                OverdraftLimit = overdraftLimit
            });
        }

        public void SetTransferLimit(decimal transferLimit, CorrelatedMessage source)
        {
            if (transferLimit < 0)
                throw new InvalidOperationException("Transfer limit must be >= 0");

            this.Raise(new TransferLimitIsSetEvent(source)
            {
                AccountId = this.Id,
                TransferLimit = transferLimit
            });
        }

        public void TryDepositAmount(decimal amount, CorrelatedMessage source)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Amount to deposit must be > 0");

            this.Raise(new AmountDepositedEvent(source)
                {
                    AccountId = this.Id,
                    Amount = amount

                });
        }

        public void TryWithdrawAmount(decimal amount, CorrelatedMessage source)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Amount to withdraw must be > 0");

            this.Raise(new AmountWithdrawnEvent(source)
            {
                AccountId = this.Id,
                Amount = amount

            });
        }

        public void TryDepositCheque(decimal amount, CorrelatedMessage source)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Amount to deposit a cheque must be > 0");

            this.Raise(new ChequeDepositedEvent(source)
            {
                AccountId = this.Id,
                Amount = amount,
                //ChequeDepositDate = clock.GetCurrentInstant()
            });
        }


        public void SetupAccountDaily(CorrelatedMessage source)
        {
            //reset used_transfer_limit
            _used_transfer_limit = 0;

            //clear pending amount
            if (_pending_amount > 0)
            {
                this.Raise(new ChequesClearedEvent(source)
                {
                    AccountId = this.Id,
                    Amount = _pending_amount

                });
            }
        }


    }
}

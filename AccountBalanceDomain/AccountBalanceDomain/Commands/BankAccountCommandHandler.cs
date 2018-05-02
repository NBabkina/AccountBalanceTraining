using System;
using System.Collections.Generic;
using AccountBalanceDomain;
using AccountBalanceDomain.Commands;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;

namespace AccountBalanceTest 
{
    public class BankAccountCommandHandler : IDisposable, 
        IHandleCommand<CreateBankAccountCommand>,
        IHandleCommand<SetOverdraftLimitCommand>,
        IHandleCommand<SetTransferLimitCommand>,
        IHandleCommand<DepositCashCommand>,
        IHandleCommand<WithdrawCashCommand>
    {
        private IDispatcher _bus;
        private readonly IRepository _repo;
        private readonly IDisposable _sub;

        public BankAccountCommandHandler(IRepository repo, IDispatcher bus)
        {
            this._bus = bus;
            this._repo = repo;

            this._sub = new CompositeDisposable
            {
                //subscribe the bus to our commands
                bus.Subscribe<CreateBankAccountCommand>(this),
                bus.Subscribe<SetOverdraftLimitCommand>(this),
                bus.Subscribe<SetTransferLimitCommand>(this),
                bus.Subscribe<DepositCashCommand>(this),
                bus.Subscribe<WithdrawCashCommand>(this)
            };
        }

        public void Dispose()
        {
            this._sub.Dispose();
        }

        public CommandResponse Handle(CreateBankAccountCommand command)
        {
            if (_repo.TryGetById<BankAccount>(command.AccountId, out var acc))
            {
                throw new InvalidOperationException("Account already exists");
            }

            var account = BankAccount.Create(command.AccountId, command.AccountHolderName, command);
            _repo.Save(account);

            return command.Succeed();
        }


        public CommandResponse Handle(SetOverdraftLimitCommand command)
        {
            if (!_repo.TryGetById<BankAccount>(command.AccountId, out var account))
                throw new InvalidOperationException("Account does not exist");

            account.SetOverdraftLimit(command.OverdraftLimit, command);

            _repo.Save(account);

            return command.Succeed();
        }

        public CommandResponse Handle(SetTransferLimitCommand command)
        {
            if (!_repo.TryGetById<BankAccount>(command.AccountId, out var account))
                throw new InvalidOperationException("Account does not exist");

            account.SetTransferLimit(command.TransferLimit, command);

            _repo.Save(account);

            return command.Succeed();
        }

        public CommandResponse Handle(DepositCashCommand command)
        {
            if (!_repo.TryGetById<BankAccount>(command.AccountId, out var account))
                throw new InvalidOperationException("Account does not exist");

            account.TryDepositAmount(command.Amount, command);

            _repo.Save(account);

            return command.Succeed();
        }

        public CommandResponse Handle(WithdrawCashCommand command)
        {
            if (!_repo.TryGetById<BankAccount>(command.AccountId, out var account))
                throw new InvalidOperationException("Account does not exist");

            account.TryWithdrawAmount(command.Amount, command);

            _repo.Save(account);

            return command.Succeed();
        }




    }




        /// <summary>
        /// This class exists because ReactiveDomain forces its version of Rx on us, even though
        /// the bits of it we use do not even use Rx.
        /// </summary>
        sealed class CompositeDisposable : List<IDisposable>, IDisposable
        {
            public void Dispose()
            {
                foreach (var disp in this)
                {
                    try
                    {
                        disp?.Dispose();
                    }
                    catch
                    {
                        // Ignore
                    }
                }
            }
        }
}
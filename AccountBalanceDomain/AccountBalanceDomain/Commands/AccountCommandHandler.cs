using System;
using AccountBalanceDomain;
using AccountBalanceDomain.Commands;
using NodaTime;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using System.Collections.Generic;

namespace AccountBalanceTest
{
    public class AccountCommandHandler : IDisposable, 
        IHandleCommand<CreateAccountCommand>,
        IHandleCommand<SetOverdraftLimitCommand>,
        IHandleCommand<SetTransferLimitCommand>,
        IHandleCommand<DepositCashCommand>,
        IHandleCommand<WithdrawCashCommand>,
        IHandleCommand<DepositChequeCommand>,
        IHandleCommand<SetupAccountDailyCommand>,
        IHandleCommand<WithdrawWireTransferCommand>
    {
        private IDispatcher _bus;
        private readonly IRepository _repo;
        private readonly IDisposable _sub;
        private IClock _clock;

        public IClock Clock
        {
            set => _clock = value;
        }

        public AccountCommandHandler(IRepository repo, IDispatcher bus)
        {
            this._bus = bus;
            this._repo = repo;

            this._sub = new CompositeDisposable
            {
                //subscribe the bus to our commands
                bus.Subscribe<CreateAccountCommand>(this),
                bus.Subscribe<SetOverdraftLimitCommand>(this),
                bus.Subscribe<SetTransferLimitCommand>(this),
                bus.Subscribe<DepositCashCommand>(this),
                bus.Subscribe<WithdrawCashCommand>(this),
                bus.Subscribe<DepositChequeCommand>(this),
                bus.Subscribe<SetupAccountDailyCommand>(this),
                bus.Subscribe<WithdrawWireTransferCommand>(this)
            };
        }

        public void Dispose()
        {
            this._sub.Dispose();
        }

        public CommandResponse Handle(CreateAccountCommand command)
        {
            if (_repo.TryGetById<Account>(command.AccountId, out var acc))
            {
                throw new InvalidOperationException("Account already exists");
            }

            var account = Account.Create(command.AccountId, command.AccountHolderName, command);
            _repo.Save(account);

            return command.Succeed();
        }


        public CommandResponse Handle(SetOverdraftLimitCommand command)
        {
            if (!_repo.TryGetById<Account>(command.AccountId, out var account))
                throw new InvalidOperationException("Account does not exist");

            account.SetOverdraftLimit(command.OverdraftLimit, command);

            _repo.Save(account);

            return command.Succeed();
        }

        public CommandResponse Handle(SetTransferLimitCommand command)
        {
            if (!_repo.TryGetById<Account>(command.AccountId, out var account))
                throw new InvalidOperationException("Account does not exist");

            account.SetTransferLimit(command.TransferLimit, command);

            _repo.Save(account);

            return command.Succeed();
        }

        public CommandResponse Handle(DepositCashCommand command)
        {
            if (!_repo.TryGetById<Account>(command.AccountId, out var account))
                throw new InvalidOperationException("Account does not exist");

            account.TryDepositAmount(command.Amount, command);

            _repo.Save(account);

            return command.Succeed();
        }

        public CommandResponse Handle(WithdrawCashCommand command)
        {
            if (!_repo.TryGetById<Account>(command.AccountId, out var account))
                throw new InvalidOperationException("Account does not exist");

            account.TryWithdrawAmount(command.Amount, command);

            _repo.Save(account);

            return command.Succeed();
        }

        public CommandResponse Handle(DepositChequeCommand command)
        {
            if (!_repo.TryGetById<Account>(command.AccountId, out var account))
                throw new InvalidOperationException("Account does not exist");

            account.TryDepositCheque(command.Amount, command);

            _repo.Save(account);

            return command.Succeed();
        }

        // supposed to be called on schedule daily before 9 a.m.
        public CommandResponse Handle(SetupAccountDailyCommand command)
        {
            if (!_repo.TryGetById<Account>(command.AccountId, out var account))
                throw new InvalidOperationException("Account does not exist");

            account.SetupAccountDaily(command);

            _repo.Save(account);

            return command.Succeed();
        }

        public CommandResponse Handle(WithdrawWireTransferCommand command)
        {
            if (!_repo.TryGetById<Account>(command.AccountId, out var account))
                throw new InvalidOperationException("Account does not exist");

            account.TryWithdrawWireTransfer(command.Amount, command);

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
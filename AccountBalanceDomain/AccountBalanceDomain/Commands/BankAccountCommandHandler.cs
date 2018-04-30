using System;
using AccountBalanceDomain;
using AccountBalanceDomain.Commands;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;

namespace AccountBalanceTest 
{
    public class BankAccountCommandHandler : IHandleCommand<CreateBankAccountCommand>, IDisposable
    {
        private IDispatcher _bus;
        private readonly IRepository _repo;
        private readonly IDisposable _sub;

        public BankAccountCommandHandler(IRepository repo, IDispatcher bus)
        {
            this._bus = bus;
            this._repo = repo;
            this._sub = bus.Subscribe(this);
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
            if(! _repo.TryGetById<BankAccount>(command.AccountId, out var account))
                throw new InvalidOperationException("Account does not exist");

            //do smth

            _repo.Save(account);

            return command.Succeed();
        }

        public CommandResponse Handle(SetTransferLimitCommand command)
        {
            if (!_repo.TryGetById<BankAccount>(command.AccountId, out var account))
                throw new InvalidOperationException("Account does not exist");

            /// do smth

            _repo.Save(account);

            return command.Succeed();
        }



    }
}
using System;
using AccountBalanceDomain;
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

        public BankAccountCommandHandler(IDispatcher bus, IRepository repo)
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

            
            //command.Fail(null);
            return command.Succeed();
        }
    }
}
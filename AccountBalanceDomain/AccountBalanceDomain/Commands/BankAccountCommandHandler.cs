using System;
using AccountBalanceDomain;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;

namespace AccountBalanceTest 
{
    public class BankAccountCommandHandler : IHandleCommand<CreateAccountCommand>, IDisposable
    {
        private IDispatcher bus;
        private IRepository repo;
        private IDisposable sub;

        public BankAccountCommandHandler(IDispatcher bus, IRepository repo)
        {
            this.bus = bus;
            this.repo = repo;
            this.sub = bus.Subscribe(this);
        }

        public void Dispose()
        {
            this.sub.Dispose();
        }

        public CommandResponse Handle(CreateAccountCommand command)
        {
            //command.Fail(null);
            return command.Succeed();
            //throw new System.NotImplementedException();
        }
    }
}
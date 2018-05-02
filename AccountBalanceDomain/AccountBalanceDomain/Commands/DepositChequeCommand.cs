using System;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain.Commands
{
    public class DepositChequeCommand : Command
    {
        public Guid AccountId;
        public decimal Amount;
        
        public DepositChequeCommand()
            : base(CorrelatedMessage.NewRoot())
        {
        }

    }
}
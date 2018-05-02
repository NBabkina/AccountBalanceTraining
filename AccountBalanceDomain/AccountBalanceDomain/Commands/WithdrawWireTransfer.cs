using System;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain.Commands
{
    public class WithdrawWireTransfer : Command
    {
        public Guid AccountId;
        public decimal Amount;

        public WithdrawWireTransfer()
            : base(CorrelatedMessage.NewRoot())
        {
        }

    }
}
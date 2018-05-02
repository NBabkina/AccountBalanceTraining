using System;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain.Commands
{
    public class WithdrawWireTransferCommand : Command
    {
        public Guid AccountId;
        public decimal Amount;

        public WithdrawWireTransferCommand()
            : base(CorrelatedMessage.NewRoot())
        {
        }

    }
}
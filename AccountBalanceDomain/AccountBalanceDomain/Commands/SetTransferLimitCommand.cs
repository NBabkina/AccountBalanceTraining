using System;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain.Commands
{
    public class SetTransferLimitCommand : Command
    {
        public Guid AccountId;
        public decimal TransferLimit;

        public SetTransferLimitCommand()
            : base(CorrelatedMessage.NewRoot())
        {
        }

    }
}
using System;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain.Commands
{
    public class DepositCashCommand : Command
    {
        public Guid AccountId;
        public decimal Amount;

        public DepositCashCommand()
            : base(CorrelatedMessage.NewRoot())
        {
        }

    }
}
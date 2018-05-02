using System;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain.Commands
{
    public class WithdrawCashCommand : Command
    {
        public Guid AccountId;
        public decimal Amount;

        public WithdrawCashCommand()
            : base(CorrelatedMessage.NewRoot())
        {
        }

    }
}
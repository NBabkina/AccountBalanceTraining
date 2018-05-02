using System;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain.Commands
{
    public class SetupAccountDailyCommand : Command
    {
        public Guid AccountId;

        public SetupAccountDailyCommand()
            : base(CorrelatedMessage.NewRoot())
        {
        }
    }
}
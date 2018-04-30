using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain.Commands
{
    public class SetOverdraftLimitCommand : Command
    {
        public Guid AccountId;
        public decimal OverdraftLimit;

        public SetOverdraftLimitCommand(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        {
        }

    }
}

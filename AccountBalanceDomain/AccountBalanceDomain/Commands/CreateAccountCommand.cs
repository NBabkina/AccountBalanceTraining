using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.Projections.Core.Messages;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain
{
    public class CreateAccountCommand : Command
    {
        public Guid AccountId;
        public string AccountHolderName;

        public CreateAccountCommand(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        {
        }

    }

}

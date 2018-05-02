using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain.Events
{
    public class AccountCreatedEvent : Event
    {
        public Guid AccountId { get; set; }
        public string AccountHolderName { get; set; }

        public AccountCreatedEvent(CorrelatedMessage source)
            : base(source)
        {
        }

        [JsonConstructor]
        public AccountCreatedEvent(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

    }
}

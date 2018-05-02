using System;
using Newtonsoft.Json;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain.Events
{
    public class AccountBlockedEvent : Event
    {
        public Guid AccountId { get; set; }

        public AccountBlockedEvent(CorrelatedMessage source)
            : base(source)
        {
        }

        [JsonConstructor]
        public AccountBlockedEvent(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

    }
}
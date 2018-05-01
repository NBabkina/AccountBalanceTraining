using System;
using Newtonsoft.Json;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain.Events
{
    public class OverdraftLimitIsSetEvent : Event
    {
        public Guid AccountId { get; set; }
        public decimal OverdraftLimit { get; set; }

        public OverdraftLimitIsSetEvent(CorrelatedMessage source)
            : base(source)
        {
        }

        [JsonConstructor]
        public OverdraftLimitIsSetEvent(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

    }
}
using System;
using Newtonsoft.Json;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain.Events
{
    public class AmountWithdrawnEvent : Event
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }

        public AmountWithdrawnEvent(CorrelatedMessage source)
            : base(source)
        {
        }

        [JsonConstructor]
        public AmountWithdrawnEvent(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

    }
}
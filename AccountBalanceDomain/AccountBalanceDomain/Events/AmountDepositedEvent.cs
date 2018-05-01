using System;
using Newtonsoft.Json;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain.Events
{
    public class AmountDepositedEvent : Event
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }

        public AmountDepositedEvent(CorrelatedMessage source)
            : base(source)
        {
        }

        [JsonConstructor]
        public AmountDepositedEvent(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

    }
}
using System;
using Newtonsoft.Json;
using ReactiveDomain.Messaging;
using NodaTime;

namespace AccountBalanceDomain.Events
{
    public class ChequeDepositedEvent : Event
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        //public Instant ChequeDepositDate { get; set; }

        public ChequeDepositedEvent(CorrelatedMessage source)
            : base(source)
        {
        }

        [JsonConstructor]
        public ChequeDepositedEvent(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

    }
}
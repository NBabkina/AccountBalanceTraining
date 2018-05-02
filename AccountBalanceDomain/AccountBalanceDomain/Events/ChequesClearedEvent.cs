using System;
using Newtonsoft.Json;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain.Events
{
    public class ChequesClearedEvent : Event
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        //public Instant ChequeDepositDate { get; set; }

        public ChequesClearedEvent(CorrelatedMessage source)
            : base(source)
        {
        }

        [JsonConstructor]
        public ChequesClearedEvent(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

    }
}
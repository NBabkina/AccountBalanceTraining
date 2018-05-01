using System;
using Newtonsoft.Json;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain.Events
{
    public class TransferLimitIsSetEvent : Event
    {
        public Guid AccountId { get; set; }
        public decimal TransferLimit { get; set; }

        public TransferLimitIsSetEvent(CorrelatedMessage source)
            : base(source)
        {
        }

        [JsonConstructor]
        public TransferLimitIsSetEvent(CorrelationId correlationId, SourceId sourceId)
            : base(correlationId, sourceId)
        { }

    }
}
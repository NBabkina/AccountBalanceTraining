using System;
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

    }
}
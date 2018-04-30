using System;
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

    }
}
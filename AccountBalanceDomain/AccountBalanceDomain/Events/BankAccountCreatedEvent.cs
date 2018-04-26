﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain.Events
{
    public class BankAccountCreatedEvent : Event
    {
        public Guid AccountId { get; set; }
        public string AccountHolderName { get; set; }

        public BankAccountCreatedEvent(CorrelatedMessage source)
            : base(source)
        {
        }

    }
}
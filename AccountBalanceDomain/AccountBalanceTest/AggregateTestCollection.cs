using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountBalanceTest
{
    using Xunit;

    [CollectionDefinition("AggregateTest")]
    public sealed class AggregateTestCollection : ICollectionFixture<EventStoreFixture>
    {
    }
}

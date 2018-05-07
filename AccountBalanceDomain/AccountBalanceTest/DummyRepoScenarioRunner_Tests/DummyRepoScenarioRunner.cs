using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountBalanceDomain;
using ReactiveDomain;
using ReactiveDomain.Foundation;
using ReactiveDomain.Messaging;
using ReactiveDomain.Messaging.Bus;
using Xunit.ScenarioReporting;

namespace AccountBalanceTest
{
    public class DummyRepoScenarioRunner<TAggregate> : ReflectionBasedScenarioRunner<Event, Command, Event>
        where TAggregate : EventDrivenStateMachine
    {
        readonly DummyRepository _repo;
        readonly Guid _id;
        readonly IDispatcher _bus;

        /// <summary>
        /// The Standard Service constructor takes a ICommandBus and IRepository
        /// The repository will be loaded with the events
        /// 
        /// </summary>
        public DummyRepoScenarioRunner(Guid aggregateId, Action<IRepository, IDispatcher> init)
        {
            _id = aggregateId;

            _bus = new Dispatcher("Test bus", watchSlowMsg: true, slowMsgThreshold: TimeSpan.FromSeconds(5),
                slowCmdThreshold: TimeSpan.FromSeconds(5));
            _repo = new DummyRepository(_id);

            init(_repo, _bus);

            this.Configure(
                configure =>
                {
                    configure.ForType<Message>(
                        ct => { ct.Ignore(m => m.MsgId); });

                    //configure.Compare<Instant>((x, y) => x.ToDateTimeUtc() == y.ToDateTimeUtc());

                    //configure.CustomReader<Instant>((instant, b, s) =>
                    //{
                    //    return new ObjectPropertyDefinition(
                    //        typeof(Instant),
                    //        s,
                    //        b,
                    //        instant,
                    //        "",
                    //        null,
                    //        new ObjectPropertyDefinition[0]);
                    //});
                });
        }

        sealed class DummyRepository : IRepository
        {
            Event[] _existingEvents;
            readonly List<Event> _newEvents = new List<Event>();
            readonly Guid _id;

            public DummyRepository(Guid aggregateId)
            {
                _id = aggregateId;
            }

            public bool TryGetById<T>(Guid id, out T aggregate) where T : class, IEventSource
            {
                aggregate = null;
                try
                {
                    aggregate = GetById<T>(id);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public bool TryGetById<T>(Guid id, int version, out T aggregate) where T : class, IEventSource
            {
                throw new NotImplementedException();
            }

            public T GetById<T>(Guid id) where T : class, IEventSource
            {
                if (typeof(T) != typeof(TAggregate))
                    throw new AggregateNotFoundException(id, typeof(T));
                if (!id.Equals(_id))
                    throw new AggregateNotFoundException(id, typeof(T));
                if (_existingEvents == null || _existingEvents.Length == 0)
                    throw new AggregateNotFoundException(id, typeof(T));

                var instance = Activator.CreateInstance(typeof(T), true);
                ((IEventSource) instance).RestoreFromEvents(_existingEvents);
                return (T) instance;
            }

            public T GetById<T>(Guid id, int version) where T : class, IEventSource
            {
                throw new NotImplementedException();
            }

            public void Save(IEventSource aggregate)
            {
                _newEvents.AddRange(aggregate.TakeEvents().OfType<Event>());
            }

            public void LoadEvents(IEnumerable<Event> events)
            {
                _existingEvents = events.ToArray();
            }

            public IReadOnlyList<Event> RecordedEvents => _newEvents;
        }

        sealed class Handler : IHandle<Message>, IDisposable
        {
            readonly List<Event> _events;
            readonly IDisposable _sub;

            public Handler(ISubscriber bus)
            {
                _sub = bus.Subscribe(this);
                _events = new List<Event>();
            }

            public void Handle(Message message)
            {
                if (message is Event ev)
                    _events.Add(ev);
            }

            public IReadOnlyList<Event> RecordedEvents => _events.ToList();

            public void Dispose() => _sub.Dispose();
        }

        protected override Task Given(IReadOnlyList<Event> givens)
        {
            if (_repo.TryGetById<TAggregate>(_id, out var existing))
                throw new InvalidOperationException();

            _repo.LoadEvents(givens);

            return Task.CompletedTask;
        }

        protected override Task When(Command when)
        {
            // If running in debugger...
            try
            {
                _bus.Send(when);
            }
            catch (CommandException ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }

            return Task.CompletedTask;
        }

        protected override Task<IReadOnlyList<Event>> ActualResults()
        {
            return Task.FromResult(_repo.RecordedEvents);
        }
    }
}
using System;
using System.Collections.Generic;
using AccountBalanceDomain.Events;
using ReactiveDomain;
using ReactiveDomain.Messaging;

namespace AccountBalanceDomain
{
    //public abstract class Aggregate
    //{
    //    public abstract Guid Id { get; }
    //    private List<Event> _unsubmittedEventsList;

    //    private Dictionary<Type, Action<Event>>
    //        _callbacks; //Key = type of event, Value = Aggregate's method with a param = event


    //    public List<Event> GetUnsubmittedEvents()
    //    {
    //        var tmp = _unsubmittedEventsList;
    //        _unsubmittedEventsList.Clear();
    //        return tmp;
    //    }


    //    protected void RaiseEvent(Event ev)
    //    {
    //        //add this event to _unsubmittedEventsList
    //        _unsubmittedEventsList.Add(ev);

    //        //perform corresponding operation
    //        var operation = _callbacks[ev.GetType()];
    //        operation.Invoke(ev);

    //    }

    //    protected void Register<T>(Action<T> operation) where T : Event
    //    {
    //        _callbacks[typeof(T)] = (x => operation((T) x));
    //    }
    //}


    public class BankAccount : EventDrivenStateMachine
    {
        //backing field for Id property
       // private Guid _id;

        //public override Guid Id => _id; //getter

        public string AccountHolderName { get; set; }


        private BankAccount()
        {
            // call Register() to store all types of operations we need to perform on each type of event
            Register<BankAccountCreatedEvent>(OnBankAccountCreatedEvent);
            //Register<MoneyAddedEvent>(OnMoneyAddedEvent);
            //Register<MoneyTakenEvent>(OnMoneyTakenEvent);
        }

        public static BankAccount Create(Guid id, string accountHolderName)
        {
            var acc = new BankAccount();

            //acc.RaiseEvent(new BankAccountCreatedEvent {AccountId = id, AccountHolderName = accountHolderName});

            return acc;
        }

        public void OnBankAccountCreatedEvent(BankAccountCreatedEvent ev)
        {
            Id = ev.AccountId;
            AccountHolderName = ev.AccountHolderName;
            Console.WriteLine("bank account created: " + ev.AccountId);
        }


    }
}

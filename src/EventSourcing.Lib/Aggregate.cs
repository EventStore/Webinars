using System.Collections.Generic;
using System.Linq;

namespace EventSourcing.Lib {
    public abstract class Aggregate {
        public IReadOnlyCollection<object> Changes => _changes.AsReadOnly();

        public void ClearChanges() => _changes.Clear();

        public int Version { get; protected set; } = -1;

        readonly List<object> _changes = new();
        
        public abstract void Load(IEnumerable<object> events);

        public abstract string GetId();

        protected void AddChange(object evt) => _changes.Add(evt);
        
        protected void EnsureDoesntExist() {
            if (Version > -1) throw new DomainException($"Booking already exists: {GetId()}");
        }
        
        protected void EnsureExists() {
            if (Version == -1) throw new DomainException($"Booking doesn't exist: {GetId()}");
        }
    }
    
    public abstract class Aggregate<T> : Aggregate where T : AggregateState<T>, new() {
        protected void Apply(object evt) {
            AddChange(evt);
            State = State.When(evt);
        }

        public override void Load(IEnumerable<object> events) {
            State = events.Aggregate(new T(), Fold);

            T Fold(T state, object evt) {
                Version++;
                return state.When(evt);
            }
        }

        public T State { get; private set; }
    }
    
    public abstract class Aggregate<T, TId> : Aggregate<T>
        where T : AggregateState<T, TId>, new() 
        where TId : AggregateId {
        public override string GetId() => State.Id;
    }
}

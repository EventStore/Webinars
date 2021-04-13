// ReSharper disable ReturnTypeCanBeEnumerable.Global

using System.Collections.Generic;

namespace EventSourcing.Lib {
    public abstract class Aggregate<TId, TState> where TId : AggregateId where TState : AggregateState<TId> {
        public TState State { get; set; }

        readonly List<object> _changes = new();

        public IReadOnlyCollection<object> Changes => _changes.AsReadOnly();

        protected void Apply(object evt) {
            _changes.Add(evt);
            ChangeState(When(evt));
        }

        protected abstract TState When(object evt);

        protected void ChangeState(TState state)
            => State = state with {
                Version = State.Version + 1
            };

        protected void EnsureExists() {
            if (State.Version < 0) throw new DomainException($"{GetType().Name} {State.Id} doesn't exist");
        }

        protected void EnsureDoesntExist() {
            if (State.Version > -1) throw new DomainException($"{GetType().Name} {State.Id} already exist");
        }
    }
}

// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace EventSourcing.Lib {
    public abstract class Aggregate<TId, TState> where TId : AggregateId where TState : AggregateState<TId> {
        public TState State { get; set; }

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

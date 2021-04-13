using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Lib {
    public interface IAggregateStore {
        Task Store<T, TId, TState>(T aggregate, CancellationToken cancellationToken)
            where T : Aggregate<TId, TState> where TId : AggregateId where TState : AggregateState<TId>;

        Task<T> Load<T, TId, TState>(TId id, CancellationToken cancellationToken)
            where T : Aggregate<TId, TState>, new() where TId : AggregateId where TState : AggregateState<TId>;

        Task<bool> Exists<T, TId, TState>(TId id, CancellationToken cancellationToken)
            where T : Aggregate<TId, TState> where TId : AggregateId where TState : AggregateState<TId>;
    }

    public class OptimisticConcurrencyException<TState, TId> : Exception where TState : AggregateState<TId> where TId : AggregateId {
        public OptimisticConcurrencyException(TState state)
            : base($"State version mismatch {typeof(TState).Name}:{state.Id}:{state.Version}") { }
    }
}

using System.Collections.Generic;

namespace EventSourcing.Lib {
    public abstract record Result<T, TState, TId>(T State)
        where T : Aggregate<TState, TId>
        where TState : AggregateState<TState, TId>, new()
        where TId : AggregateId { }

    public record OkResult<T, TState, TId>(T State, IEnumerable<object> Changes) : Result<T, TState, TId>(State) 
        where T : Aggregate<TState, TId> 
        where TState : AggregateState<TState, TId>, new()
        where TId : AggregateId { }
}

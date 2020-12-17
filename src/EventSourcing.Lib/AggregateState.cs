namespace EventSourcing.Lib {
    public abstract record AggregateState<T> where T : AggregateState<T> {
        public abstract T When(object @event);
    }
    
    public abstract record AggregateState<T, TId> : AggregateState<T>
        where T : AggregateState<T, TId>
        where TId : AggregateId {
        public TId Id { get; protected init; }
    }
}

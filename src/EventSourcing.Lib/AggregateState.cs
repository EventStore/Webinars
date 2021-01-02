namespace EventSourcing.Lib {
    public abstract record AggregateState<TId> : Document where TId : AggregateId {
        public int Version { get; init; } = -1;
    }
}

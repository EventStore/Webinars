namespace EventSourcing.Lib {
    public abstract record AggregateState<TId> : Document where TId : AggregateId {
        public int Version { get; init; } = -1;

        int _initialVersion = -1;
        
        public void SetInitialVersion() => _initialVersion = Version;

        public bool InitialVersionMatches(int expected) => _initialVersion == expected;
    }
}

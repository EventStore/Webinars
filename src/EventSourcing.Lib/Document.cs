namespace EventSourcing.Lib {
    public abstract record Document {
        public string Id { get; set; }
    }
}

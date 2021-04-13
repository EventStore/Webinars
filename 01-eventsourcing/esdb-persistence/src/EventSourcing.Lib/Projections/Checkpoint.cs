namespace EventSourcing.Lib.Projections {
    public record Checkpoint(string Id, long? Position);
}

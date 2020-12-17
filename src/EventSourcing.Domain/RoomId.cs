using EventSourcing.Lib;

namespace EventSourcing.Domain {
    public record RoomId(string Value) : AggregateId(Value) { }
}

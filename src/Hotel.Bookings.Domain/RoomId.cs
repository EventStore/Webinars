using EventSourcing.Lib;

namespace Hotel.Bookings.Domain {
    public record RoomId(string Value) : AggregateId(Value) { }
}

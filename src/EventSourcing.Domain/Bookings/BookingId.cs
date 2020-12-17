using EventSourcing.Lib;

namespace EventSourcing.Domain.Bookings {
    public record BookingId(string Value) : AggregateId(Value) { }
}

using EventSourcing.Lib;

namespace Hotel.Bookings.Domain.Bookings {
    public record BookingId(string Value) : AggregateId(Value) { }
}

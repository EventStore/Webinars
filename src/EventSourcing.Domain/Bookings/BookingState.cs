using EventSourcing.Lib;
using static EventSourcing.Domain.Bookings.BookingEvents;

namespace EventSourcing.Domain.Bookings {
    public record BookingState : AggregateState<BookingState, BookingId> {
        string              GuestId     { get; init; }
        string              RoomId      { get; init; }
        internal StayPeriod Period      { get; init; }
        internal Money      Price       { get; init; }
        internal Money      Outstanding { get; init; }
        BookingStatus       Status      { get; init; }
        bool                Paid        { get; init; }

        public override BookingState When(object @event)
            => @event switch {
                V1.RoomBooked e =>
                    this with {
                        Id = new BookingId(e.BookingId),
                        GuestId = e.GuestId,
                        RoomId = e.RoomId,
                        Price = new Money {Amount = e.Price, Currency = e.Currency},
                        Outstanding = new Money {Amount = e.Price, Currency = e.Currency},
                        Status = BookingStatus.Booked },
                V1.BookingPaid e =>
                    this with {
                        Status = Paid ? BookingStatus.Paid : Status },
                _ => this
            };
    }
}

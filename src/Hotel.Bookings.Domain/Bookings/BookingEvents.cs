using System;

namespace Hotel.Bookings.Domain.Bookings {
    public static class BookingEvents {
        public record RoomBooked(
            string         BookingId,
            string         RoomId,
            string         GuestId,
            DateTimeOffset CheckInDate,
            DateTimeOffset CheckOutDate,
            float          Price,
            string         Currency,
            string         BookedBy,
            DateTimeOffset BookedAt
        );

        public record BookingPaid(
            string         BookingId,
            float          AmountPaid,
            string         Currency,
            bool           IsFullyPaid,
            float          Outstanding,
            string         PaidBy,
            DateTimeOffset PaidAt
        );

        public record DiscountApplied(
            string BookingId,
            float  Discount,
            string Currency,
            float  Outstanding,
            bool   IsFullyPaid
        );
    }
}

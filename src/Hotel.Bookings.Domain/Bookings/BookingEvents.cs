using System;

namespace EventSourcing.Domain.Bookings {
    public static class BookingEvents {
        public static class V1 {
            public record RoomBooked(
                string         BookingId,
                string         GuestId,
                string         RoomId,
                DateTimeOffset CheckIn,
                DateTimeOffset CheckOut,
                double         Price,
                string         Currency,
                string         BookedBy,
                DateTimeOffset BookedAt
            );

            public record BookingExtended(
                string         BookingId,
                DateTimeOffset CheckOut,
                string         ExtendedBy,
                DateTimeOffset ExtendedAt
            );

            public record BookingPaid(
                string         BookingId,
                double         AmountPaid,
                double         AmountOutstanding,
                string         PaidBy,
                DateTimeOffset PaidAt
            );
        }

        public static class V2 {
            public record BookingExtended(
                string         BookingId,
                string         GuestId,
                DateTimeOffset CheckOut,
                string         ExtendedBy,
                DateTimeOffset ExtendedAt
            );

            public record BookingShrunk(
                string         BookingId,
                string         GuestId,
                DateTimeOffset CheckOut,
                string         ExtendedBy,
                DateTimeOffset ExtendedAt
            );
        }
    }
}

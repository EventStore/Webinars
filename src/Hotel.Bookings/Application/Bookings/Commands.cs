using System;

namespace Hotel.Bookings.Application.Bookings {
    public static class BookingCommands {
        public record Book(
            string         BookingId,
            string         RoomId,
            string         GuestId,
            DateTimeOffset From,
            DateTimeOffset To,
            double         Price,
            string         Currency,
            string         BookedBy,
            DateTimeOffset BookedAt
        );

        public record RecordPayment(
            string         BookingId,
            double         Amount,
            string         Currency,
            string         PaidBy,
            DateTimeOffset PaidAt
        );
    }
}

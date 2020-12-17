using System;

namespace EventSourcing.Application.Bookings {
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

        public class Pay {
            public string BookingId { get; set; }
            public bool   Paid      { get; set; }
        }
    }
}

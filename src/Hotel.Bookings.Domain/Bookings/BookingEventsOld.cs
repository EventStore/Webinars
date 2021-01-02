using System;

namespace EventSourcing.Domain.Bookings
{
    public static class BookingEventsOld
    {
        public class BookingCreated
        {
            public string BookingId { get; set; }
            public string HotelId { get; set; }
            public string CustomerId { get; set; }
            public DateTimeOffset From { get; set; }
            public DateTimeOffset To { get; set; }
        }

        public class BookingPaid
        {
            public string BookingId { get; set; }
            public bool PaidStatus { get; set; }
        }
    }
}

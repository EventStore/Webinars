using EventSourcing.Lib;
using static Hotel.Bookings.Domain.Bookings.BookingEvents;

namespace Hotel.Bookings.Application.Bookings {
    public static class BookingEventMappings {
        public static void MapEvents() {
            TypeMap.AddType<RoomBooked>("RoomBooked");
            TypeMap.AddType<BookingPaid>("BookingPaid");
            TypeMap.AddType<DiscountApplied>("DiscountApplied");
        }
    }
}

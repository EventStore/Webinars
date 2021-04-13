using System;
using EventSourcing.Lib;

namespace Hotel.Bookings.Domain {
    public record StayPeriod {
        public DateTimeOffset CheckIn  { get; internal init; }
        public DateTimeOffset CheckOut { get; internal init; }
        
        internal StayPeriod() { }

        public StayPeriod(DateTimeOffset checkIn, DateTimeOffset checkOut) {
            if (checkIn > checkOut) throw new DomainException("Check in date must be before check out date");

            (CheckIn, CheckOut) = (checkIn, checkOut);
        }
    }
}

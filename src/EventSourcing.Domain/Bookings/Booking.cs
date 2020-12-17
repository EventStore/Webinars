using System;
using System.Threading.Tasks;
using EventSourcing.Lib;
using static EventSourcing.Domain.Bookings.BookingEvents;
using static EventSourcing.Domain.Services;

namespace EventSourcing.Domain.Bookings {
    public class Booking : Aggregate<BookingState, BookingId> {
        public async Task BookRoom(
            BookingId       bookingId,
            string          guestId,
            RoomId          roomId,
            StayPeriod      period,
            Money           price,
            string          bookedBy,
            DateTimeOffset  bookedAt,
            IsRoomAvailable isRoomAvailable
        ) {
            EnsureDoesntExist();
            await EnsureRoomAvailable(roomId, period, isRoomAvailable);

            Apply(
                new V1.RoomBooked(
                    bookingId,
                    guestId,
                    roomId,
                    period.CheckIn,
                    period.CheckOut,
                    price.Amount,
                    price.Currency,
                    bookedBy,
                    bookedAt
                )
            );
        }

        public void RecordPayment(
            Money           paid,
            ConvertCurrency convertCurrency,
            string          paidBy,
            DateTimeOffset  paidAt
        ) {
            EnsureExists();

            var localPaid = State.Price.IsSameCurrency(paid)
                ? paid
                : convertCurrency(paid, State.Price.Currency);
            var outstanding = State.Outstanding - localPaid;

            Apply(new V1.BookingPaid(State.Id, localPaid, outstanding, paidBy, paidAt));
        }

        static async Task EnsureRoomAvailable(RoomId roomId, StayPeriod period, IsRoomAvailable isRoomAvailable) {
            var roomAvailable = await isRoomAvailable(roomId, period);
            if (!roomAvailable) throw new DomainException("Room not available");
        }
    }

    public enum BookingStatus {
        Booked,
        Confirmed,
        Paid,
        Cancelled
    }
}

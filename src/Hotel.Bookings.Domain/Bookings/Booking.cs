using System;
using System.Threading.Tasks;
using EventSourcing.Lib;
using static Hotel.Bookings.Domain.Services;

namespace Hotel.Bookings.Domain.Bookings {
    public class Booking : Aggregate<BookingId, BookingState> {
        public Booking() => State = new BookingState();
        
        public async Task BookRoom(
            BookingId                bookingId,
            string                   guestId,
            RoomId                   roomId,
            StayPeriod               period,
            Money                    price,
            string                   bookedBy,
            DateTimeOffset           bookedAt,
            IsRoomAvailable isRoomAvailable
        ) {
            EnsureDoesntExist();
            await EnsureRoomAvailable(roomId, period, isRoomAvailable);

            ChangeState(new BookingState {
                Id          = bookingId.Value,
                RoomId      = roomId,
                GuestId     = guestId,
                Price       = price,
                Outstanding = price,
                Period      = period,
                Paid        = false
            });
        }

        public void RecordPayment(
            Money                    paid,
            ConvertCurrency convertCurrency,
            string                   paidBy,
            DateTimeOffset           paidAt
        ) {
            EnsureExists();

            var localPaid = State.Price.IsSameCurrency(paid)
                ? paid
                : convertCurrency(paid, State.Price.Currency);
            var outstanding = State.Outstanding - localPaid;

            ChangeState(State with {
                Outstanding = outstanding,
                Paid = outstanding.Amount == 0
            });
        }

        static async Task EnsureRoomAvailable(RoomId roomId, StayPeriod period, IsRoomAvailable isRoomAvailable) {
            var roomAvailable = await isRoomAvailable(roomId, period);
            if (!roomAvailable) throw new DomainException("Room not available");
        }
    }
}

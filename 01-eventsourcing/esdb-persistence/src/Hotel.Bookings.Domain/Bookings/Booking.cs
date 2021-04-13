using System;
using System.Threading.Tasks;
using EventSourcing.Lib;
using static Hotel.Bookings.Domain.Bookings.BookingEvents;
using static Hotel.Bookings.Domain.Services;

namespace Hotel.Bookings.Domain.Bookings {
    public class Booking : Aggregate<BookingId, BookingState> {
        public Booking() => State = new BookingState();

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
                new RoomBooked(
                    bookingId.Value,
                    roomId.Value,
                    guestId,
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

            Apply(
                new BookingPaid(
                    State.Id,
                    paid.Amount,
                    paid.Currency,
                    outstanding.Amount == 0,
                    outstanding.Amount,
                    paidBy,
                    paidAt
                )
            );
        }

        public void ApplyDiscount(
            Money           discount,
            ConvertCurrency convertCurrency
        ) {
            EnsureExists();

            var localDiscountAmount = State.Price.IsSameCurrency(discount)
                ? discount
                : convertCurrency(discount, State.Price.Currency);
            var outstanding = State.Outstanding - localDiscountAmount;

            Apply(
                new DiscountApplied(
                    State.Id,
                    discount.Amount,
                    discount.Currency,
                    outstanding.Amount,
                    outstanding.Amount == 0
                )
            );
        }

        static async Task EnsureRoomAvailable(RoomId roomId, StayPeriod period, IsRoomAvailable isRoomAvailable) {
            var roomAvailable = await isRoomAvailable(roomId, period);
            if (!roomAvailable) throw new DomainException("Room not available");
        }

        public override BookingState When(object evt)
            => evt switch {
                RoomBooked e =>
                    new BookingState {
                        Id          = e.BookingId,
                        RoomId      = new RoomId(e.RoomId),
                        GuestId     = e.GuestId,
                        Price       = new Money {Amount       = e.Price, Currency       = e.Currency},
                        Outstanding = new Money {Amount       = e.Price, Currency       = e.Currency},
                        Period      = new StayPeriod {CheckIn = e.CheckInDate, CheckOut = e.CheckOutDate},
                        Paid        = false
                    },
                BookingPaid e =>
                    State with {
                        Outstanding = new Money {Amount = e.Outstanding, Currency = State.Price.Currency},
                        Paid = e.IsFullyPaid
                    },
                DiscountApplied e =>
                    State with {
                        Outstanding = new Money {Amount = e.Outstanding, Currency = State.Price.Currency},
                        Paid = e.IsFullyPaid
                    },
                _ => State
            };
    }
}

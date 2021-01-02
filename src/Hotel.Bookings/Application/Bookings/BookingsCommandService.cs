using EventSourcing.Lib;
using Hotel.Bookings.Domain;
using Hotel.Bookings.Domain.Bookings;
using static Hotel.Bookings.Domain.Services;

namespace Hotel.Bookings.Application.Bookings {
    public class BookingsCommandService : ApplicationService<Booking, BookingId, BookingState> {
        public BookingsCommandService(IAggregateStore store, IsRoomAvailable isRoomAvailable, ConvertCurrency convertCurrency) : base(store) {
            OnNew<BookingCommands.Book>(
                (booking, cmd) =>
                    booking.BookRoom(
                        new BookingId(cmd.BookingId),
                        cmd.GuestId,
                        new RoomId(cmd.RoomId),
                        new StayPeriod(cmd.From, cmd.To),
                        new Money(cmd.Price, cmd.Currency),
                        cmd.BookedBy,
                        cmd.BookedAt,
                        isRoomAvailable
                    )
            );

            OnExisting<BookingCommands.RecordPayment>(
                cmd => new BookingId(cmd.BookingId),
                (booking, cmd) => booking.RecordPayment(
                    new Money(cmd.Amount, cmd.Currency),
                    convertCurrency,
                    cmd.PaidBy,
                    cmd.PaidAt
                )
            );
        }
    }
}

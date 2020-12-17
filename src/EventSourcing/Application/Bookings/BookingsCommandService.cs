using System;
using EventSourcing.Domain;
using EventSourcing.Domain.Bookings;
using EventSourcing.Lib;
using static EventSourcing.Domain.Services;

namespace EventSourcing.Application.Bookings {
    public class BookingsCommandService : ApplicationService<Booking, BookingState, BookingId> {

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

            OnExisting<BookingCommands.Pay>(
                cmd => new BookingId(cmd.BookingId),
                (booking, cmd) => booking.RecordPayment(cmd.Paid)
            );
        }
    }
}

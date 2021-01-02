using System.Threading;
using System.Threading.Tasks;
using Hotel.Bookings.Application.Bookings;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Bookings.HttpApi.Bookings {
    [Route("/booking")]
    public class CommandApi : ControllerBase {
        readonly BookingsCommandService _service;

        public CommandApi(BookingsCommandService service) => _service = service;

        [HttpPost]
        [Route("")]
        public Task Book([FromBody] BookingCommands.Book cmd, CancellationToken cancellationToken) 
            => _service.HandleNew(cmd, cancellationToken);

        [HttpPost]
        [Route("pay")]
        public Task ConfirmPayment([FromBody] BookingCommands.RecordPayment cmd, CancellationToken cancellationToken) 
            => _service.HandleExisting(cmd, cancellationToken);
    }
}

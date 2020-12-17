using System.Threading.Tasks;
using EventSourcing.Application.Bookings;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcing.HttpApi
{
    [Route("/booking")]
    public class CommandApi : ControllerBase {
        readonly BookingsCommandService _service;
        
        public CommandApi(BookingsCommandService service) => _service = service;

        [HttpPost]
        [Route("")]
        public Task Book([FromBody] BookingCommands.Book cmd) => _service.HandleNew(cmd);

        [HttpPost]
        [Route("pay")]
        public Task ConfirmPayment([FromBody] BookingCommands.Pay cmd) => _service.HandleExisting(cmd);
    }
}

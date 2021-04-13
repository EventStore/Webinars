using System.Threading.Tasks;

namespace EventSourcing.Lib.Projections {
    public interface IEventHandler {
        Task HandleEvent(object evt);
    }
}

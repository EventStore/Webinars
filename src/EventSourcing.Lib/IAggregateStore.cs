using System.Threading.Tasks;

namespace EventSourcing.Lib
{
    public interface IAggregateStore {
        Task Store<T>(T entity) where T : Aggregate;

        Task<T> Load<T>(string id) where T : Aggregate, new();

        Task<bool> Exists<T>(string id) where T : Aggregate;
    }
}

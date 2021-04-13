using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Lib.Projections {
    public interface ICheckpointStore {
        ValueTask<Checkpoint> GetLastCheckpoint(string checkpointId, CancellationToken cancellationToken = default);

        ValueTask<Checkpoint> StoreCheckpoint(Checkpoint checkpoint, CancellationToken cancellationToken = default);
    }
}

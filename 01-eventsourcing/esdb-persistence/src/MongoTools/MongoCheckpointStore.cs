using System.Threading;
using System.Threading.Tasks;
using DotNext.Lib;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DotNext.Infrastructure.MongoDb
{
    public class MongoCheckpointStore : ICheckpointStore {
        readonly ILogger<MongoCheckpointStore> Log;

        public MongoCheckpointStore(IMongoCollection<Checkpoint> database, ILogger<MongoCheckpointStore> logger) {
            Checkpoints = database;
            Log         = logger;
        }

        public MongoCheckpointStore(IMongoDatabase database, ILogger<MongoCheckpointStore> logger) : this(database.GetCollection<Checkpoint>("checkpoint"), logger) { }

        IMongoCollection<Checkpoint> Checkpoints { get; }

        public async ValueTask<Checkpoint> GetLastCheckpoint(string checkpointId, CancellationToken cancellationToken = default) {
            Log.LogDebug("[{CheckpointId}] Finding checkpoint...", checkpointId);

            var checkpoint = await Checkpoints.AsQueryable()
                .Where(x => x.Id == checkpointId)
                .SingleOrDefaultAsync(cancellationToken);

            if (checkpoint is null) {
                checkpoint = new Checkpoint(checkpointId);
                Log.LogInformation("[{CheckpointId}] Checkpoint not found. Defaulting to earliest position.", checkpointId);
            }
            else {
                Log.LogInformation("[{CheckpointId}] Checkpoint found at position {Checkpoint}", checkpointId, checkpoint.Position);
            }

            return checkpoint;
        }

        public async ValueTask<Checkpoint> StoreCheckpoint(Checkpoint checkpoint, CancellationToken cancellationToken = default) {
            await Checkpoints.ReplaceOneAsync(
                x => x.Id == checkpoint.Id,
                checkpoint,
                MongoDefaults.DefaultReplaceOptions,
                cancellationToken
            );

            Log.LogDebug("[{CheckpointId}] Checkpoint position set to {Checkpoint}", checkpoint.Id, checkpoint.Position);

            return checkpoint;
        }
    }
}

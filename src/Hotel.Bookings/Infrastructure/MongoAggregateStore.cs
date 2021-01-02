using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Lib;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoTools;

namespace Hotel.Bookings.Infrastructure {
    public class MongoAggregateStore : IAggregateStore {
        readonly IMongoDatabase               _database;
        readonly ILogger<MongoAggregateStore> _logger;

        public MongoAggregateStore(IMongoDatabase database, ILogger<MongoAggregateStore> logger) {
            _database = database;
            _logger   = logger;
        }

        public async Task Store<T, TId, TState>(T aggregate, CancellationToken cancellationToken)
            where T : Aggregate<TId, TState> where TId : AggregateId where TState : AggregateState<TId> {
            var expectNew = aggregate.State.InitialVersionMatches(-1);

            foreach (var change in aggregate.Changes) {
                _logger.LogInformation("Change: @{Event}", change);
            }
            
            var result = await _database.ReplaceDocument(
                aggregate.State,
                cfg => cfg.IsUpsert = expectNew,
                cancellationToken
            );
            var success = expectNew ? result.UpsertedId != null : result.ModifiedCount > 0;
            if (!success) throw new OptimisticConcurrencyException<TState, TId>(aggregate.State);
        }

        public async Task<T> Load<T, TId, TState>(TId id, CancellationToken cancellationToken)
            where T : Aggregate<TId, TState>, new() where TId : AggregateId where TState : AggregateState<TId> {
            var state = await _database.LoadDocument<TState>(id.Value, cancellationToken);
            state.SetInitialVersion();
            var aggregate = new T {State = state};
            return aggregate;
        }

        public Task<bool> Exists<T, TId, TState>(TId id, CancellationToken cancellationToken)
            where T : Aggregate<TId, TState> where TId : AggregateId where TState : AggregateState<TId>
            => _database.DocumentExists<TState>(id.Value, cancellationToken);
    }
}

using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Lib;
using EventStore.Client;

namespace Hotel.Bookings.Infrastructure {
    public class EsDbAggregateStore : IAggregateStore {
        readonly EventStoreClient _client;

        public EsDbAggregateStore(EventStoreClient client) => _client = client;

        public async Task Store<T, TId, TState>(T aggregate, CancellationToken cancellationToken)
            where T : Aggregate<TId, TState> where TId : AggregateId where TState : AggregateState<TId> {
            var expectNew = aggregate.State.Version == -1;

            var stream = StreamName<T, TId, TState>(aggregate);

            var dbEvents = aggregate.Changes
                .Select(
                    x => new EventData(
                        Uuid.NewUuid(),
                        TypeMap.GetTypeName(x.GetType()),
                        JsonSerializer.SerializeToUtf8Bytes(x)
                    )
                );

            var op = expectNew
                ? _client.AppendToStreamAsync(stream, StreamState.NoStream, dbEvents, cancellationToken: cancellationToken)
                : _client.AppendToStreamAsync(
                    stream,
                    StreamRevision.FromInt64(aggregate.State.Version),
                    dbEvents,
                    cancellationToken: cancellationToken
                );

            await op;
        }

        public async Task<T> Load<T, TId, TState>(TId id, CancellationToken cancellationToken)
            where T : Aggregate<TId, TState>, new() where TId : AggregateId where TState : AggregateState<TId> {
            var stream = StreamName<T, TId, TState>(id);

            var read = _client.ReadStreamAsync(
                Direction.Forwards,
                stream,
                StreamPosition.Start,
                cancellationToken: cancellationToken
            );

            var aggregate = new T();

            await foreach (var e in read) {
                var evt = e.Deserialize();
                aggregate.State = aggregate.When(evt) with {Version = aggregate.State.Version + 1};
            }

            return aggregate;
        }

        public Task<bool> Exists<T, TId, TState>(TId id, CancellationToken cancellationToken)
            where T : Aggregate<TId, TState> where TId : AggregateId where TState : AggregateState<TId>
            => throw new System.NotImplementedException();

        static string StreamName<T, TId, TState>(T aggregate)
            where T : Aggregate<TId, TState> where TId : AggregateId where TState : AggregateState<TId>
            => $"{typeof(T).Name}-{aggregate.State.Id}";

        static string StreamName<T, TId, TState>(TId id)
            where T : Aggregate<TId, TState> where TId : AggregateId where TState : AggregateState<TId>
            => $"{typeof(T).Name}-{id.Value}";
    }
}

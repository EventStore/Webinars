using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Lib {
    public class ApplicationService<T, TId, TState>
        where T : Aggregate<TId, TState>, new() where TId : AggregateId where TState : AggregateState<TId> {
        readonly IAggregateStore                     _store;
        readonly Dictionary<Type, Action<T, object>> _actions = new();
        readonly Dictionary<Type, Func<object, TId>> _getId   = new();

        protected ApplicationService(IAggregateStore store) => _store = store;

        protected void OnNew<TCommand>(Action<T, TCommand> action)
            => _actions.Add(typeof(TCommand), (aggregate, cmd) => action(aggregate, (TCommand) cmd));

        protected void OnExisting<TCommand>(Func<TCommand, TId> getId, Action<T, TCommand> action) {
            _actions.Add(typeof(TCommand), (aggregate, cmd) => action(aggregate, (TCommand) cmd));
            _getId.Add(typeof(TCommand), cmd => getId((TCommand) cmd));
        }

        public async Task HandleNew<TCommand>(TCommand cmd, CancellationToken cancellationToken) {
            var aggregate = new T();
            _actions[typeof(TCommand)](aggregate, cmd);
            await _store.Store<T, TId, TState>(aggregate, cancellationToken);
        }

        public async Task HandleExisting<TCommand>(TCommand cmd, CancellationToken cancellationToken) {
            var id        = _getId[typeof(TCommand)](cmd);
            var aggregate = await _store.Load<T, TId, TState>(id, cancellationToken);
            _actions[typeof(TCommand)](aggregate, cmd);
            await _store.Store<T, TId, TState>(aggregate, cancellationToken);
        }
    }
}

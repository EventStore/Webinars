using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Lib {
    public class CommandService<T, TId, TState>
        where T : Aggregate<TId, TState>, new() where TId : AggregateId where TState : AggregateState<TId> {
        readonly IAggregateStore                         _store;
        readonly Dictionary<Type, Func<T, object, Task>> _actions = new();
        readonly Dictionary<Type, Func<object, TId>>     _getId   = new();

        protected CommandService(IAggregateStore store) => _store = store;

        protected void OnNew<TCommand>(Func<T, TCommand, Task> action)
            => _actions.Add(typeof(TCommand), (aggregate, cmd) => action(aggregate, (TCommand) cmd));

        protected void OnNew<TCommand>(Action<T, TCommand> action)
            => _actions.Add(typeof(TCommand), (aggregate, cmd) => Sync(() => action(aggregate, (TCommand) cmd)));

        protected void OnExisting<TCommand>(Func<TCommand, TId> getId, Func<T, TCommand, Task> action) {
            _actions.Add(typeof(TCommand), (aggregate, cmd) => action(aggregate, (TCommand) cmd));
            _getId.Add(typeof(TCommand), cmd => getId((TCommand) cmd));
        }

        protected void OnExisting<TCommand>(Func<TCommand, TId> getId, Action<T, TCommand> action) {
            _actions.Add(typeof(TCommand), (aggregate, cmd) => Sync(() => action(aggregate, (TCommand) cmd)));
            _getId.Add(typeof(TCommand), cmd => getId((TCommand) cmd));
        }

        public async Task HandleNew<TCommand>(TCommand cmd, CancellationToken cancellationToken) {
            var aggregate = new T();
            await _actions[typeof(TCommand)](aggregate, cmd);
            await _store.Store<T, TId, TState>(aggregate, cancellationToken);
        }

        public async Task HandleExisting<TCommand>(TCommand cmd, CancellationToken cancellationToken) {
            var id        = _getId[typeof(TCommand)](cmd);
            var aggregate = await _store.Load<T, TId, TState>(id, cancellationToken);
            await _actions[typeof(TCommand)](aggregate, cmd);
            await _store.Store<T, TId, TState>(aggregate, cancellationToken);
        }

        static Task Sync(Action action) {
            action();
            return Task.CompletedTask;
        }
    }
}

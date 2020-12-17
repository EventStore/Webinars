using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventSourcing.Lib {
    public abstract class ApplicationService<T, TState, TId>
        where T : Aggregate<TState, TId>, new()
        where TState : AggregateState<TState, TId>, new()
        where TId : AggregateId {
        readonly IAggregateStore                     _store;
        readonly Dictionary<Type, Action<T, object>> _handleNew      = new();
        readonly Dictionary<Type, Action<T, object>> _handleExisting = new();
        readonly Dictionary<Type, Func<object, TId>> _getId          = new();

        protected ApplicationService(IAggregateStore store) => _store = store;

        protected void OnNew<TCommand>(Action<T, TCommand> action) where TCommand : class
            => _handleNew.Add(typeof(TCommand), (aggregate, cmd) => action(aggregate, (TCommand) cmd));

        protected void OnExisting<TCommand>(Func<TCommand, TId> getId, Action<T, TCommand> action) where TCommand : class {
            _handleExisting.Add(typeof(TCommand), (aggregate, cmd) => action(aggregate, (TCommand) cmd));
            _getId.TryAdd(typeof(TCommand), cmd => getId((TCommand) cmd));
        }

        public Task HandleNew<TCommand>(TCommand command) => Handle(command, false);

        public Task HandleExisting<TCommand>(TCommand command) => Handle(command, false);

        async Task Handle<TCommand>(TCommand command, bool onExisting) {
            var handlerMap = onExisting ? _handleExisting : _handleNew;

            if (!handlerMap.TryGetValue(typeof(TCommand), out var action)) {
                throw new Exceptions.CommandHandlerNotFound(typeof(TCommand));
            }

            var aggregate = onExisting ? await Load() : new T();

            action(aggregate, command);

            await _store.Store(aggregate);

            async Task<T> Load() {
                var id     = _getId[typeof(TCommand)](command);
                var loaded = await _store.Load<T>(id);
                if (loaded == null) throw new Exceptions.AggregateNotFound<T>(id);

                return loaded;
            }
        }
    }
}

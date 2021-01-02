using System.Text.Json;
using EventSourcing.Lib;
using EventStore.Client;

namespace Hotel.Bookings.Infrastructure {
    public static class EventDeserializer {
        public static object Deserialize(this ResolvedEvent resolvedEvent) {
            var dataType = TypeMap.GetType(resolvedEvent.Event.EventType);
            var data     = JsonSerializer.Deserialize(resolvedEvent.Event.Data.Span, dataType);
            return data;
        }
    }
}

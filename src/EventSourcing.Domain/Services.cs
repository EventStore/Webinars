using System.Threading.Tasks;

namespace EventSourcing.Domain {
    public static class Services {
        public delegate Task<bool> IsRoomAvailable(RoomId roomId, StayPeriod period);

        public delegate Money ConvertCurrency(Money from, string targetCurrency);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.MatchingEngine.Connector.Messages;
using Lykke.MatchingEngine.Connector.Models;

namespace Lykke.Job.RabbitTradeMonitoring.Services
{
    public class MessageStatistic
    {
        private readonly object _gate = new object();
        private Dictionary<string, Stat> _instrumentIndex = new Dictionary<string, Stat>();
        private Dictionary<string, Stat> _clientIndex = new Dictionary<string, Stat>();

        public void HandleMessage(LimitOrderMessage message)
        {
            lock (_gate)
            {
                foreach (var order in message.Orders)
                {
                    if (order.Order == null) continue;

                    if (!_instrumentIndex.ContainsKey(order.Order.AssetPairId))
                        _instrumentIndex[order.Order.AssetPairId] = new Stat(order.Order.AssetPairId);
                    UpdateStat(_instrumentIndex[order.Order.AssetPairId], order);

                    if (!_clientIndex.ContainsKey(order.Order.ClientId))
                        _clientIndex[order.Order.ClientId] = new Stat(order.Order.ClientId);
                    UpdateStat(_clientIndex[order.Order.ClientId], order);
                }
            }
        }

        private static void UpdateStat(Stat stat, LimitOrderMessage.LimitOrder order)
        {
            stat.CountOrderMessage++;
            stat.AddOrderStatus(order.Order.Status);

            if (order.Trades != null)
                stat.CountTrade += order.Trades.Count;
        }

        public Dictionary<string, Stat> GetInstrumentStatAndReset()
        {
            lock (_gate)
            {
                var data = _instrumentIndex;
                _instrumentIndex = new Dictionary<string, Stat>();
                return data;
            }
        }

        public Dictionary<string, Stat> GetClientStatAndReset()
        {
            lock (_gate)
            {
                var data = _clientIndex;
                _clientIndex = new Dictionary<string, Stat>();
                return data;
            }
        }
    }

    public class Stat
    {
        public Stat(string key)
        {
            Key = key;
        }

        public string Key { get; }
        public int CountOrderMessage { get; set; }
        public int CountTrade { get; set; }
        public Dictionary<OrderStatus, int> CountOrderMessageByStatus { get; } = new Dictionary<OrderStatus, int>();

        public void AddOrderStatus(OrderStatus status)
        {
            if (!CountOrderMessageByStatus.ContainsKey(status))
                CountOrderMessageByStatus[status] = 1;
            else
                CountOrderMessageByStatus[status]++;
        }
    }
}

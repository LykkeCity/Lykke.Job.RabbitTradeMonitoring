using System;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.Job.RabbitTradeMonitoring.Services;
using Lykke.MatchingEngine.Connector.Models.RabbitMq;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Job.RabbitTradeMonitoring.RabbitSubscribers
{
    public class MyRabbitSubscriber : IStartable, IStopable
    {
        private readonly ILog _log;
        private readonly MessageStatistic _messageStatistic;
        private readonly string _connectionString;
        private readonly string _exchangeName;
        private RabbitMqSubscriber<LimitOrders> _subscriber;

        public MyRabbitSubscriber(
            ILog log,
            MessageStatistic messageStatistic,
            string connectionString,
            string exchangeName)
        {
            _log = log;
            _messageStatistic = messageStatistic;
            _connectionString = connectionString;
            _exchangeName = exchangeName;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .CreateForSubscriber(_connectionString, _exchangeName, "rabbittrademonitoring");
            settings.ExchangeName = _exchangeName;
            settings.QueueName = $"{_exchangeName}.rabbit-trade-monitoring";
            settings.IsDurable = false;

            _subscriber = new RabbitMqSubscriber<LimitOrders>(settings,
                    new ResilientErrorHandlingStrategy(_log, settings,
                        retryTimeout: TimeSpan.FromSeconds(10),
                        next: new DeadQueueErrorHandlingStrategy(_log, settings)))
                .SetMessageDeserializer(new JsonMessageDeserializer<LimitOrders>())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .SetLogger(_log)
                .SetConsole(new LogToConsole())
                .Start();
        }

        private async Task ProcessMessageAsync(LimitOrders arg)
        {
            _messageStatistic.HandleMessage(arg);

            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }
    }
}

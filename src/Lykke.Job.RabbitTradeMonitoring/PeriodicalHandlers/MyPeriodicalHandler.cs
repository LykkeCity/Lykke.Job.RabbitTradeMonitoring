using System;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Job.RabbitTradeMonitoring.Services;
using Lykke.Job.RabbitTradeMonitoring.Settings.JobSettings;

namespace Lykke.Job.RabbitTradeMonitoring.PeriodicalHandlers
{
    public class MyPeriodicalHandler : TimerPeriod
    {
        private readonly ILog _log;
        private readonly MessageStatistic _messageStatistic;
        private readonly RabbitTradeMonitoringSettings _settings;

        public MyPeriodicalHandler(ILog log, MessageStatistic messageStatistic, RabbitTradeMonitoringSettings settings) :
            // TODO: Sometimes, it is enough to hardcode the period right here, but sometimes it's better to move it to the settings.
            // Choose the simplest and sufficient solution
            base(nameof(MyPeriodicalHandler), (int)settings.TimeoutForReport.TotalMilliseconds, log)
        {
            _log = log;
            _messageStatistic = messageStatistic;
            _settings = settings;
        }

        public override async Task Execute()
        {
            var pairstat = _messageStatistic.GetInstrumentStatAndReset();
            var clientstat = _messageStatistic.GetInstrumentStatAndReset();

            foreach (var pair in pairstat)
            {
                await _log.WriteInfoAsync("RabbitTradeMonitoring", "StatByInstruments", pair.ToJson(), "Statitic by event with instrument");
            }

            foreach (var pair in clientstat.OrderByDescending(e => e.Value.CountOrderMessage).Take(_settings.TopCountClient))
            {
                await _log.WriteInfoAsync("RabbitTradeMonitoring", "StatByClients", pair.ToJson(), "Statitic by event with client");
            }

            await _log.WriteInfoAsync("RabbitTradeMonitoring", "StatByEvents", 
                new {
                    CountEvent = pairstat.Sum(e => e.Value.CountOrderMessage),
                    CountClient = clientstat.Count,
                    CountInstumrnt = pairstat.Count
                }.ToJson(), "Statitic by event");


            await Task.CompletedTask;
        }
    }
}

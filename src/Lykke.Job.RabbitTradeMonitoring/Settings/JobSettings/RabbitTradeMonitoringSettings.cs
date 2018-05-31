using System;

namespace Lykke.Job.RabbitTradeMonitoring.Settings.JobSettings
{
    public class RabbitTradeMonitoringSettings
    {
        public DbSettings Db { get; set; }
        public RabbitMqSettings LykkeTrade { get; set; }
        public int TopCountClient { get; set; }
        public TimeSpan TimeoutForReport { get; set; }
    }
}

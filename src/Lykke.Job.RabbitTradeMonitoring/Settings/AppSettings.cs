using Lykke.Job.RabbitTradeMonitoring.Settings.JobSettings;
using Lykke.Job.RabbitTradeMonitoring.Settings.SlackNotifications;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.RabbitTradeMonitoring.Settings
{
    public class AppSettings
    {
        public RabbitTradeMonitoringSettings RabbitTradeMonitoringJob { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }

        [Optional]
        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }
    }
}

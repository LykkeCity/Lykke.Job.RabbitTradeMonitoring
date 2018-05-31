using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.RabbitTradeMonitoring.Settings.JobSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}

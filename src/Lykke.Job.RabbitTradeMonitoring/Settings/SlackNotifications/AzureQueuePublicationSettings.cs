﻿namespace Lykke.Job.RabbitTradeMonitoring.Settings.SlackNotifications
{
    public class AzureQueuePublicationSettings
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }
}

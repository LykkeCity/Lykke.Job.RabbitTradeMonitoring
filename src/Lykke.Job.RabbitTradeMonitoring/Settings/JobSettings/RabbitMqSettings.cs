﻿using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.RabbitTradeMonitoring.Settings.JobSettings
{
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string ConnectionString { get; set; }

        public string ExchangeName { get; set; }
    }
}

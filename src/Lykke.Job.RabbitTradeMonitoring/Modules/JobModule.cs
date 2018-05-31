using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Job.RabbitTradeMonitoring.Core.Services;
using Lykke.Job.RabbitTradeMonitoring.Settings.JobSettings;
using Lykke.Job.RabbitTradeMonitoring.Services;
using Lykke.SettingsReader;
using Lykke.Job.RabbitTradeMonitoring.PeriodicalHandlers;
using Lykke.Job.RabbitTradeMonitoring.RabbitSubscribers;

namespace Lykke.Job.RabbitTradeMonitoring.Modules
{
    public class JobModule : Module
    {
        private readonly RabbitTradeMonitoringSettings _settings;
        private readonly IReloadingManager<RabbitTradeMonitoringSettings> _settingsManager;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public JobModule(RabbitTradeMonitoringSettings settings, IReloadingManager<RabbitTradeMonitoringSettings> settingsManager, ILog log)
        {
            _settings = settings;
            _log = log;
            _settingsManager = settingsManager;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // NOTE: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            // builder.RegisterType<QuotesPublisher>()
            //  .As<IQuotesPublisher>()
            //  .WithParameter(TypedParameter.From(_settings.Rabbit.ConnectionString))

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.RegisterInstance(_settings)
                .SingleInstance();

            RegisterPeriodicalHandlers(builder);

            RegisterRabbitMqSubscribers(builder);

            // TODO: Add your dependencies here

            builder.Populate(_services);
        }

        private void RegisterPeriodicalHandlers(ContainerBuilder builder)
        {
            // TODO: You should register each periodical handler in DI container as IStartable singleton and autoactivate it

            builder.RegisterType<MyPeriodicalHandler>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();
        }

        private void RegisterRabbitMqSubscribers(ContainerBuilder builder)
        {
            // TODO: You should register each subscriber in DI container as IStartable singleton and autoactivate it

            builder.RegisterType<MyRabbitSubscriber>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance()
                .WithParameter("connectionString", _settings.LykkeTrade.ConnectionString)
                .WithParameter("exchangeName", _settings.LykkeTrade.ExchangeName);

            builder.RegisterType<MessageStatistic>()
                .SingleInstance();
        }
    }
}

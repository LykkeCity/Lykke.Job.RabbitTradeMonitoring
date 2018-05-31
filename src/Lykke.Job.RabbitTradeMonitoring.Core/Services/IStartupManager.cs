using System.Threading.Tasks;

namespace Lykke.Job.RabbitTradeMonitoring.Core.Services
{
    public interface IStartupManager
    {
        Task StartAsync();
    }
}
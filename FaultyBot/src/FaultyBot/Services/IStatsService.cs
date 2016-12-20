using System.Threading.Tasks;

namespace FaultyBot.Services
{
    public interface IStatsService
    {
        Task<string> Print();
        Task Reset();
    }
}

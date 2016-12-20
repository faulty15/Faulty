using FaultyBot.Services.Database.Models;

namespace FaultyBot.Services.Database.Repositories
{
    public interface IBotConfigRepository : IRepository<BotConfig>
    {
        BotConfig GetOrCreate();
    }
}

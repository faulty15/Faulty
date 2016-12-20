using FaultyBot.Services.Database.Models;
using System.Collections.Generic;

namespace FaultyBot.Services.Database.Repositories
{
    public interface IClashOfClansRepository : IRepository<ClashWar>
    {
        IEnumerable<ClashWar> GetAllWars();
    }
}

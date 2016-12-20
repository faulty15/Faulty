using FaultyBot.Services.Database.Models;
using System.Collections.Generic;

namespace FaultyBot.Services.Database.Repositories
{
    public interface IDonatorsRepository : IRepository<Donator>
    {
        IEnumerable<Donator> GetDonatorsOrdered();
        Donator AddOrUpdateDonator(ulong userId, string name, int amount);
    }
}

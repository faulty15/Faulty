using FaultyBot.Services.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace FaultyBot.Services.Database.Repositories.Impl
{
    public class RepeaterRepository : Repository<Repeater>, IRepeaterRepository
    {
        public RepeaterRepository(DbContext context) : base(context)
        {
        }
    }
}

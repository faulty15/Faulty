using FaultyBot.Services.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace FaultyBot.Services.Database.Repositories.Impl
{
    public class CustomReactionsRepository : Repository<CustomReaction>, ICustomReactionRepository
    {
        public CustomReactionsRepository(DbContext context) : base(context)
        {
        }
    }
}

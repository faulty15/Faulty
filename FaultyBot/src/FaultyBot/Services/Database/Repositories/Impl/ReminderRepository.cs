using FaultyBot.Services.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace FaultyBot.Services.Database.Repositories.Impl
{
    public class ReminderRepository : Repository<Reminder>, IReminderRepository
    {
        public ReminderRepository(DbContext context) : base(context)
        {
        }
    }
}

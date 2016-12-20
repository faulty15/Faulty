using FaultyBot.Services.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace FaultyBot.Services.Database.Repositories.Impl
{
    public class CurrencyTransactionsRepository : Repository<CurrencyTransaction>, ICurrencyTransactionsRepository
    {
        public CurrencyTransactionsRepository(DbContext context) : base(context)
        {
        }
    }
}

﻿using FaultyBot.Services.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaultyBot.Services.Database.Repositories
{
    public interface IQuoteRepository : IRepository<Quote>
    {
        IEnumerable<Quote> GetAllQuotesByKeyword(ulong guildId, string keyword);
        Task<Quote> GetRandomQuoteByKeywordAsync(ulong guildId, string keyword);
        IEnumerable<Quote> GetGroup(int skip, int take);
    }
}

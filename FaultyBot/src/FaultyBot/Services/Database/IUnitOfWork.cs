using FaultyBot.Services.Database.Repositories;
using System;
using System.Threading.Tasks;

namespace FaultyBot.Services.Database
{
    public interface IUnitOfWork : IDisposable
    {
        FaultyContext _context { get; }

        IQuoteRepository Quotes { get; }
        IGuildConfigRepository GuildConfigs { get; }
        IDonatorsRepository Donators { get; }
        IClashOfClansRepository ClashOfClans { get; }
        IReminderRepository Reminders { get; }
        ISelfAssignedRolesRepository SelfAssignedRoles { get; }
        IBotConfigRepository BotConfig { get; }
        IRepeaterRepository Repeaters { get; }
        IUnitConverterRepository ConverterUnits { get; }
        ICustomReactionRepository CustomReactions { get; }
        ICurrencyRepository Currency { get; }
        ICurrencyTransactionsRepository CurrencyTransactions { get; }
        IMusicPlaylistRepository MusicPlaylists { get; }

        int Complete();
        Task<int> CompleteAsync();
    }
}

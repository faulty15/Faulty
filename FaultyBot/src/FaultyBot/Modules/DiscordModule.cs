using Discord.Commands;
using FaultyBot.Services;
using NLog;

namespace FaultyBot.Modules
{
    public class DiscordModule
    {
        protected ILocalization _l { get; }
        protected CommandService _commands { get; }
        protected ShardedDiscordClient  _client { get; }
        protected Logger _log { get; }
        protected string _prefix { get; }

        public DiscordModule(ILocalization loc, CommandService cmds, ShardedDiscordClient client)
        {
            string prefix;
            if (FaultyBot.ModulePrefixes.TryGetValue(this.GetType().Name, out prefix))
                _prefix = prefix;
            else
                _prefix = "?missing_prefix?";

            _l = loc;
            _commands = cmds;
            _client = client;
            _log = LogManager.GetCurrentClassLogger();
        }
    }
}

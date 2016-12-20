using Discord;
using Discord.Commands;
using FaultyBot.Attributes;
using FaultyBot.Extensions;
using FaultyBot.Services;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace FaultyBot.Modules.Administration
{
    public partial class Administration
    {
        [Group]
        public class CrossServerTextChannel
        {
            public CrossServerTextChannel()
            {
                _log = LogManager.GetCurrentClassLogger();
                FaultyBot.Client.MessageReceived += (imsg) =>
                {
                    if (imsg.Author.IsBot)
                        return Task.CompletedTask;

                    var msg = imsg as IUserMessage;
                    if (msg == null)
                        return Task.CompletedTask;

                    var channel = imsg.Channel as ITextChannel;
                    if (channel == null)
                        return Task.CompletedTask;

                    Task.Run(async () =>
                    {
                        if (msg.Author.Id == FaultyBot.Client.GetCurrentUser().Id) return;
                        foreach (var subscriber in Subscribers)
                        {
                            var set = subscriber.Value;
                            if (!set.Contains(msg.Channel))
                                continue;
                            foreach (var chan in set.Except(new[] { channel }))
                            {
                                try { await chan.SendMessageAsync(GetText(channel.Guild, channel, (IGuildUser)msg.Author, msg)).ConfigureAwait(false); } catch (Exception ex) { _log.Warn(ex); }
                            }
                        }
                    });
                    return Task.CompletedTask;
                };
            }

            private string GetText(IGuild server, ITextChannel channel, IGuildUser user, IUserMessage message) =>
                $"**{server.Name} | {channel.Name}** `{user.Username}`: " + message.Content;
            
            public static readonly ConcurrentDictionary<int, ConcurrentHashSet<ITextChannel>> Subscribers = new ConcurrentDictionary<int, ConcurrentHashSet<ITextChannel>>();
            private Logger _log { get; }

            [FaultyCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [OwnerOnly]
            public async Task Scsc(IUserMessage msg)
            {
                var channel = (ITextChannel)msg.Channel;
                var token = new FaultyRandom().Next();
                var set = new ConcurrentHashSet<ITextChannel>();
                if (Subscribers.TryAdd(token, set))
                {
                    set.Add(channel);
                    await ((IGuildUser)msg.Author).SendMessageAsync("This is your CSC token:" + token.ToString()).ConfigureAwait(false);
                }
            }

            [FaultyCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequirePermission(GuildPermission.ManageGuild)]
            public async Task Jcsc(IUserMessage imsg, int token)
            {
                var channel = (ITextChannel)imsg.Channel;

                ConcurrentHashSet<ITextChannel> set;
                if (!Subscribers.TryGetValue(token, out set))
                    return;
                set.Add(channel);
                await channel.SendMessageAsync(":ok:").ConfigureAwait(false);
            }

            [FaultyCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequirePermission(GuildPermission.ManageGuild)]
            public async Task Lcsc(IUserMessage imsg)
            {
                var channel = (ITextChannel)imsg.Channel;

                foreach (var subscriber in Subscribers)
                {
                    subscriber.Value.TryRemove(channel);
                }
                await channel.SendMessageAsync(":ok:").ConfigureAwait(false);
            }
        }
    }
}
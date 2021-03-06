﻿using Discord;
using Discord.Commands;
using FaultyBot.Attributes;
using FaultyBot.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaultyBot.Modules.Administration
{
    public partial class Administration
    {
        [Group]
        public class DMForwardCommands
        {
            private static bool ForwardDMs { get; set; }
            private static bool ForwardDMsToAllOwners { get; set; }
            
            static DMForwardCommands()
            {
                using (var uow = DbHandler.UnitOfWork())
                {
                    var config = uow.BotConfig.GetOrCreate();
                    ForwardDMs = config.ForwardMessages;
                    ForwardDMsToAllOwners = config.ForwardToAllOwners;
                }
            }

            [FaultyCommand, Usage, Description, Aliases]
            [OwnerOnly]
            public async Task ForwardMessages(IUserMessage imsg)
            {
                var channel = imsg.Channel;

                using (var uow = DbHandler.UnitOfWork())
                {
                    var config = uow.BotConfig.GetOrCreate();
                    ForwardDMs = config.ForwardMessages = !config.ForwardMessages;
                    uow.Complete();
                }
                if (ForwardDMs)
                    await channel.SendMessageAsync("✅ **I will forward DMs from now on.**").ConfigureAwait(false);
                else
                    await channel.SendMessageAsync("🆗 **I will stop forwarding DMs from now on.**").ConfigureAwait(false);
            }

            [FaultyCommand, Usage, Description, Aliases]
            [OwnerOnly]
            public async Task ForwardToAll(IUserMessage imsg)
            {
                var channel = imsg.Channel;

                using (var uow = DbHandler.UnitOfWork())
                {
                    var config = uow.BotConfig.GetOrCreate();
                    ForwardDMsToAllOwners = config.ForwardToAllOwners = !config.ForwardToAllOwners;
                    uow.Complete();
                }
                if (ForwardDMsToAllOwners)
                    await channel.SendMessageAsync("ℹ️ **I will forward DMs to all owners.**").ConfigureAwait(false);
                else
                    await channel.SendMessageAsync("ℹ️ **I will forward DMs only to the first owner.**").ConfigureAwait(false);

            }

            public static async Task HandleDMForwarding(IMessage msg, List<IDMChannel> ownerChannels)
            {
                if (ForwardDMs && ownerChannels.Any())
                {
                    var toSend = $"```markdown\n I received a message from [{msg.Author}]({msg.Author.Id}): {msg.Content}```";
                    if (ForwardDMsToAllOwners)
                    {
                        var msgs = await Task.WhenAll(ownerChannels.Where(ch => ch.Recipient.Id != msg.Author.Id)
                                                                   .Select(ch => ch.SendMessageAsync(toSend))).ConfigureAwait(false);
                    }
                    else
                    {
                        var firstOwnerChannel = ownerChannels.First();
                        if (firstOwnerChannel.Recipient.Id != msg.Author.Id)
                            try { await firstOwnerChannel.SendMessageAsync(toSend).ConfigureAwait(false); } catch { }
                    }
                }
            }
        }
    }
}

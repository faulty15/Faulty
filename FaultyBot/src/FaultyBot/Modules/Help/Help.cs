﻿using Discord.Commands;
using FaultyBot.Extensions;
using System.Linq;
using Discord;
using FaultyBot.Services;
using System.Threading.Tasks;
using FaultyBot.Attributes;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace FaultyBot.Modules.Help
{
    [FaultyModule("Help", "-")]
    public partial class Help : DiscordModule
    {
        private static string helpString { get; }
        public static string HelpString => String.Format(helpString, FaultyBot.Credentials.ClientId, FaultyBot.ModulePrefixes[typeof(Help).Name]);

        public static string DMHelpString { get; }

        static Help()
        {
            using (var uow = DbHandler.UnitOfWork())
            {
                var config = uow.BotConfig.GetOrCreate();
                helpString = config.HelpString;
                DMHelpString = config.DMHelpString;
            }
        }

        public Help(ILocalization loc, CommandService cmds, ShardedDiscordClient client) : base(loc, cmds, client)
        {
        }

        [FaultyCommand, Usage, Description, Aliases]
        public async Task Modules(IUserMessage umsg)
        {

            await umsg.Channel.SendMessageAsync("📜 **List of modules:** ```css\n• " + string.Join("\n• ", _commands.Modules.Select(m => m.Name)) + $"\n``` ℹ️ **Type** `-commands module_name` **to get a list of commands in that module.** ***e.g.*** `-commands games`")
                                       .ConfigureAwait(false);
        }

        [FaultyCommand, Usage, Description, Aliases]
        public async Task Commands(IUserMessage umsg, [Remainder] string module = null)
        {
            var channel = umsg.Channel;

            module = module?.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(module))
                return;
            var cmds = _commands.Commands.Where(c => c.Module.Name.ToUpperInvariant().StartsWith(module))
                                                  .OrderBy(c => c.Text)
                                                  .Distinct(new CommandTextEqualityComparer())
                                                  .AsEnumerable();

            var cmdsArray = cmds as Command[] ?? cmds.ToArray();
            if (!cmdsArray.Any())
            {
                await channel.SendMessageAsync("🚫 **That module does not exist.**").ConfigureAwait(false);
                return;
            }
            if (module != "customreactions" && module != "conversations")
            {
                await channel.SendTableAsync("📃 **List Of Commands:**\n", cmdsArray, el => $"{el.Text,-15} {"["+el.Aliases.Skip(1).FirstOrDefault()+"]",-8}").ConfigureAwait(false);
            }
            else
            {
                await channel.SendMessageAsync("📃 **List Of Commands:**\n• " + string.Join("\n• ", cmdsArray.Select(c => $"{c.Text}")));
            }
            await channel.SendMessageAsync($"ℹ️ **Type** `\"{FaultyBot.ModulePrefixes[typeof(Help).Name]}h CommandName\"` **to see the help for that specified command.** ***e.g.*** `-h >8ball`").ConfigureAwait(false);
        }

        [FaultyCommand, Usage, Description, Aliases]
        public async Task H(IUserMessage umsg, [Remainder] string comToFind = null)
        {
            var channel = umsg.Channel;

            comToFind = comToFind?.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(comToFind))
            {
                IMessageChannel ch = channel is ITextChannel ? await ((IGuildUser)umsg.Author).CreateDMChannelAsync() : channel;
                await ch.SendMessageAsync(HelpString).ConfigureAwait(false);
                return;
            }
            var com = _commands.Commands.FirstOrDefault(c => c.Text.ToLowerInvariant() == comToFind || c.Aliases.Select(a=>a.ToLowerInvariant()).Contains(comToFind));

            if (com == null)
            {
                await channel.SendMessageAsync("🔍 **I can't find that command.**");
                return;
            }
            var str = $"**__Help for:__ `{com.Text}`**";
            var alias = com.Aliases.Skip(1).FirstOrDefault();
            if (alias != null)
                str += $" / `{alias}`";
            if (com != null)
                await channel.SendMessageAsync(str + $@"{Environment.NewLine}**Desc:** {string.Format(com.Summary, com.Module.Prefix)} {GetCommandRequirements(com)}
**Usage:** {string.Format(com.Remarks, com.Module.Prefix)}").ConfigureAwait(false);
        }

        private string GetCommandRequirements(Command cmd)
        {
            return String.Join(" ", cmd.Source.CustomAttributes
                      .Where(ca => ca.AttributeType == typeof(OwnerOnlyAttribute) || ca.AttributeType == typeof(RequirePermissionAttribute))
                      .Select(ca =>
                      {
                          if (ca.AttributeType == typeof(OwnerOnlyAttribute))
                              return "**Bot Owner only.**";
                          else if (ca.AttributeType == typeof(RequirePermissionAttribute))
                              return $"**Requires {(GuildPermission)ca.ConstructorArguments.FirstOrDefault().Value} server permission.**".Replace("Guild", "Server");
                          else
                              return $"**Requires {(GuildPermission)ca.ConstructorArguments.FirstOrDefault().Value} channel permission.**".Replace("Guild", "Server");
                      }));
        }

        [FaultyCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        [OwnerOnly]
        public Task Hgit(IUserMessage umsg)
        {
            var helpstr = new StringBuilder();
            helpstr.AppendLine("You can support the project on patreon: <https://patreon.com/Faultybot> or paypal: <https://www.paypal.me/Kwoth>\n");
            helpstr.AppendLine("##Table Of Contents");
            helpstr.AppendLine(string.Join("\n", FaultyBot.CommandService.Modules.Where(m => m.Name.ToLowerInvariant() != "help").OrderBy(m => m.Name).Prepend(FaultyBot.CommandService.Modules.FirstOrDefault(m=>m.Name.ToLowerInvariant()=="help")).Select(m => $"- [{m.Name}](#{m.Name.ToLowerInvariant()})")));
            helpstr.AppendLine();
            string lastModule = null;
            foreach (var com in _commands.Commands.OrderBy(com=>com.Module.Name).GroupBy(c=>c.Text).Select(g=>g.First()))
            {
                if (com.Module.Name != lastModule)
                {
                    if (lastModule != null)
                    {
                        helpstr.AppendLine();
                        helpstr.AppendLine("###### [Back to TOC](#table-of-contents)");
                    }
                    helpstr.AppendLine();
                    helpstr.AppendLine("### " + com.Module.Name + "  ");
                    helpstr.AppendLine("Command and aliases | Description | Usage");
                    helpstr.AppendLine("----------------|--------------|-------");
                    lastModule = com.Module.Name;
                }
                helpstr.AppendLine($"`{com.Text}` {string.Join(" ", com.Aliases.Skip(1).Select(a=>"`"+a+"`"))} | {string.Format(com.Summary, com.Module.Prefix)} {GetCommandRequirements(com)} | {string.Format(com.Remarks, com.Module.Prefix)}");
            }
            helpstr = helpstr.Replace(FaultyBot.Client.GetCurrentUser().Username , "@BotName");
            File.WriteAllText("../../docs/Commands List.md", helpstr.ToString());
            return Task.CompletedTask;
        }

        [FaultyCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Guide(IUserMessage umsg)
        {
            var channel = (ITextChannel)umsg.Channel;

            await channel.SendMessageAsync(
@"**LIST OF COMMANDS**: <http://Faultybot.readthedocs.io/en/latest/Commands%20List/>
**Hosting Guides and docs can be found here**: <http://Faultybot.readthedocs.io/en/latest/>").ConfigureAwait(false);
        }

        [FaultyCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Donate(IUserMessage umsg)
        {
            var channel = (ITextChannel)umsg.Channel;

            await channel.SendMessageAsync(
$@"You can support the FaultyBot project on patreon. <https://patreon.com/Faultybot> or
You can send donations to `Faultydiscordbot@gmail.com`
Don't forget to leave your discord name or id in the message.

**Thank you** ♥️").ConfigureAwait(false);
        }
    }

    public class CommandTextEqualityComparer : IEqualityComparer<Command>
    {
        public bool Equals(Command x, Command y) => x.Text == y.Text;

        public int GetHashCode(Command obj) => obj.Text.GetHashCode();

    }
}
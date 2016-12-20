﻿using Discord.Commands;
using Discord;
using FaultyBot.Services;
using System.Threading.Tasks;
using FaultyBot.Attributes;
using System;
using System.Linq;
using System.Collections.Generic;
using FaultyBot.Extensions;

namespace FaultyBot.Modules.Games
{
    [FaultyModule("Games", ">")]
    public partial class Games : DiscordModule
    {
        private IEnumerable<string> _8BallResponses {
            get {
                using (var uow = DbHandler.UnitOfWork())
                {
                    return uow.BotConfig.GetOrCreate().EightBallResponses.Select(ebr => ebr.Text);
                }
            }
        }
        public Games(ILocalization loc, CommandService cmds, ShardedDiscordClient client) : base(loc, cmds, client)
        {
        }

        [FaultyCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Choose(IUserMessage umsg, [Remainder] string list = null)
        {
            var channel = (ITextChannel)umsg.Channel;
            if (string.IsNullOrWhiteSpace(list))
                return;
            var listArr = list.Split(';');
            if (listArr.Count() < 2)
                return;
            var rng = new FaultyRandom();
            await channel.SendMessageAsync(listArr[rng.Next(0, listArr.Length)]).ConfigureAwait(false);
        }

        [FaultyCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task _8Ball(IUserMessage umsg, [Remainder] string question = null)
        {
            var channel = (ITextChannel)umsg.Channel;

            if (string.IsNullOrWhiteSpace(question))
                return;
                var rng = new FaultyRandom();
            await channel.SendMessageAsync($@"❓ `Question` __**{question}**__ 
🎱 `8Ball Answers` __**{_8BallResponses.Shuffle().FirstOrDefault()}**__").ConfigureAwait(false);
        }

        [FaultyCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Rps(IUserMessage umsg, string input)
        {
            var channel = (ITextChannel)umsg.Channel;

            Func<int,string> GetRPSPick = (p) =>
            {
                if (p == 0)
                    return "🚀";
                else if (p == 1)
                    return "📎";
                else
                    return "✂️";
            };

            int pick;
            switch (input)
            {
                case "r":
                case "rock":
                case "rocket":
                    pick = 0;
                    break;
                case "p":
                case "paper":
                case "paperclip":
                    pick = 1;
                    break;
                case "scissors":
                case "s":
                    pick = 2;
                    break;
                default:
                    return;
            }
            var FaultyPick = new FaultyRandom().Next(0, 3);
            var msg = "";
            if (pick == FaultyPick)
                msg = $"It's a draw! Both picked {GetRPSPick(pick)}";
            else if ((pick == 0 && FaultyPick == 1) ||
                     (pick == 1 && FaultyPick == 2) ||
                     (pick == 2 && FaultyPick == 0))
                msg = $"{FaultyBot.Client.GetCurrentUser().Mention} won! {GetRPSPick(FaultyPick)} beats {GetRPSPick(pick)}";
            else
                msg = $"{umsg.Author.Mention} won! {GetRPSPick(pick)} beats {GetRPSPick(FaultyPick)}";

            await channel.SendMessageAsync(msg).ConfigureAwait(false);
        }

        [FaultyCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task Linux(IUserMessage umsg, string guhnoo, string loonix)
        {
            var channel = (ITextChannel)umsg.Channel;

            await channel.SendMessageAsync(
$@"I'd just like to interject for moment. What you're refering to as {loonix}, is in fact, {guhnoo}/{loonix}, or as I've recently taken to calling it, {guhnoo} plus {loonix}. {loonix} is not an operating system unto itself, but rather another free component of a fully functioning {guhnoo} system made useful by the {guhnoo} corelibs, shell utilities and vital system components comprising a full OS as defined by POSIX.

Many computer users run a modified version of the {guhnoo} system every day, without realizing it. Through a peculiar turn of events, the version of {guhnoo} which is widely used today is often called {loonix}, and many of its users are not aware that it is basically the {guhnoo} system, developed by the {guhnoo} Project.

There really is a {loonix}, and these people are using it, but it is just a part of the system they use. {loonix} is the kernel: the program in the system that allocates the machine's resources to the other programs that you run. The kernel is an essential part of an operating system, but useless by itself; it can only function in the context of a complete operating system. {loonix} is normally used in combination with the {guhnoo} operating system: the whole system is basically {guhnoo} with {loonix} added, or {guhnoo}/{loonix}. All the so-called {loonix} distributions are really distributions of {guhnoo}/{loonix}."
            ).ConfigureAwait(false);
        }
    }
}

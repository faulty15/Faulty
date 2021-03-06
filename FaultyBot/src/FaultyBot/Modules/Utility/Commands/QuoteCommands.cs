﻿using Discord;
using Discord.Commands;
using FaultyBot.Attributes;
using FaultyBot.Extensions;
using FaultyBot.Services;
using FaultyBot.Services.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaultyBot.Modules.Utility
{
    public partial class Utility
    {
        [FaultyCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task ListQuotes(IUserMessage imsg, int page = 1)
        {
            var channel = (ITextChannel)imsg.Channel;

            page -= 1;

            if (page < 0)
                return;

            IEnumerable<Quote> quotes;
            using (var uow = DbHandler.UnitOfWork())
            {
                quotes = uow.Quotes.GetGroup(page * 16, 16);
            }

            if (quotes.Any())
                await channel.SendMessageAsync($"💬 **Page {page + 1} of quotes:**\n```xl\n" + String.Join("\n", quotes.Select((q) => $"{q.Keyword,-20} by {q.AuthorName}")) + "\n```")
                             .ConfigureAwait(false);
            else
                await channel.SendMessageAsync("ℹ️ **No quotes on this page.**").ConfigureAwait(false);
        }

        [FaultyCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task ShowQuote(IUserMessage umsg, [Remainder] string keyword)
        {
            var channel = (ITextChannel)umsg.Channel;

            if (string.IsNullOrWhiteSpace(keyword))
                return;

            keyword = keyword.ToUpperInvariant();

            Quote quote;
            using (var uow = DbHandler.Instance.GetUnitOfWork())
            {
                quote = await uow.Quotes.GetRandomQuoteByKeywordAsync(channel.Guild.Id, keyword).ConfigureAwait(false);
            }

            if (quote == null)
                return;

            await channel.SendMessageAsync("📣 " + quote.Text.SanitizeMentions());
        }

        [FaultyCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task AddQuote(IUserMessage umsg, string keyword, [Remainder] string text)
        {
            var channel = (ITextChannel)umsg.Channel;

            if (string.IsNullOrWhiteSpace(keyword) || string.IsNullOrWhiteSpace(text))
                return;

            keyword = keyword.ToUpperInvariant();

            using (var uow = DbHandler.UnitOfWork())
            {
                uow.Quotes.Add(new Quote
                {
                    AuthorId = umsg.Author.Id,
                    AuthorName = umsg.Author.Username,
                    GuildId = channel.Guild.Id,
                    Keyword = keyword,
                    Text = text,
                });
                await uow.CompleteAsync().ConfigureAwait(false);
            }
            await channel.SendMessageAsync("✅ **Quote added.**").ConfigureAwait(false);
        }

        [FaultyCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        public async Task DeleteQuote(IUserMessage umsg, [Remainder] string keyword)
        {
            var channel = (ITextChannel)umsg.Channel;

            if (string.IsNullOrWhiteSpace(keyword))
                return;

            var isAdmin = ((IGuildUser)umsg.Author).GuildPermissions.Administrator;

            keyword = keyword.ToUpperInvariant();
            string response;
            using (var uow = DbHandler.UnitOfWork())
            {
                var qs = uow.Quotes.GetAllQuotesByKeyword(channel.Guild.Id, keyword);

                if (qs==null || !qs.Any())
                {
                    response = "ℹ️ **No quotes found.**";
                    return;
                }

                var q = qs.Shuffle().FirstOrDefault(elem => isAdmin || elem.AuthorId == umsg.Author.Id);

                uow.Quotes.Remove(q);
                await uow.CompleteAsync().ConfigureAwait(false);
                response = "🗑 **Deleted a random quote.**";
            }
            await channel.SendMessageAsync(response);
        }

        [FaultyCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        [RequirePermission(GuildPermission.Administrator)]
        public async Task DelAllQuotes(IUserMessage umsg, [Remainder] string keyword)
        {
            var channel = (ITextChannel)umsg.Channel;

            if (string.IsNullOrWhiteSpace(keyword))
                return;

            keyword = keyword.ToUpperInvariant();

            using (var uow = DbHandler.UnitOfWork())
            {
                var quotes = uow.Quotes.GetAllQuotesByKeyword(channel.Guild.Id, keyword);

                uow.Quotes.RemoveRange(quotes.ToArray());//wtf?!

                await uow.CompleteAsync();
            }

            await channel.SendMessageAsync($"🗑 **Deleted all quotes** with **{keyword}** keyword.");
        }
    }
}

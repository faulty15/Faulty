﻿using Discord;
using Discord.Commands;
using FaultyBot.Attributes;
using FaultyBot.Services;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FaultyBot.Modules.Searches
{
    public partial class Searches
    {
        [Group]
        public class XkcdCommands
        {
            private const string xkcdUrl = "https://xkcd.com";

            [FaultyCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [Priority(1)]
            public async Task Xkcd(IUserMessage msg, string arg = null)
            {
                var channel = (ITextChannel)msg.Channel;

                if (arg?.ToLowerInvariant().Trim() == "latest")
                {
                    using (var http = new HttpClient())
                    {
                        var res = await http.GetStringAsync($"{xkcdUrl}/info.0.json").ConfigureAwait(false);
                        var comic = JsonConvert.DeserializeObject<XkcdComic>(res);
                        var sent = await channel.SendMessageAsync($"{msg.Author.Mention} " + comic.ToString())
                                     .ConfigureAwait(false);

                        await Task.Delay(10000).ConfigureAwait(false);

                        await sent.ModifyAsync(m => m.Content = sent.Content + $"\n`Alt:` {comic.Alt}");
                    }
                    return;
                }
                await Xkcd(msg, new FaultyRandom().Next(1, 1750)).ConfigureAwait(false);
            }

            [FaultyCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [Priority(0)]
            public async Task Xkcd(IUserMessage msg, int num)
            {
                var channel = (ITextChannel)msg.Channel;

                if (num < 1)
                    return;

                using (var http = new HttpClient())
                {
                    var res = await http.GetStringAsync($"{xkcdUrl}/{num}/info.0.json").ConfigureAwait(false);

                    var comic = JsonConvert.DeserializeObject<XkcdComic>(res);
                    var sent = await channel.SendMessageAsync($"{msg.Author.Mention} " + comic.ToString())
                                 .ConfigureAwait(false);

                    await Task.Delay(10000).ConfigureAwait(false);

                    await sent.ModifyAsync(m => m.Content = sent.Content + $"\n`Alt:` {comic.Alt}");
                }
            }
        }

        public class XkcdComic
        {
            public int Num { get; set; }
            public string Month { get; set; }
            public string Year { get; set; }
            [JsonProperty("safe_title")]
            public string Title { get; set; }
            [JsonProperty("img")]
            public string ImageLink { get; set; }
            public string Alt { get; set; }

            public override string ToString() 
                => $"`Comic:` #{Num} `Title:` {Title} `Date:` {Month}/{Year}\n{ImageLink}";
        }
    }
}

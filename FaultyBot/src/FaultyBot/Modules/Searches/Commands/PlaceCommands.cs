﻿using Discord;
using Discord.Commands;
using FaultyBot.Attributes;
using FaultyBot.Services;
using System;
using System.Threading.Tasks;

namespace FaultyBot.Modules.Searches
{
    public partial class Searches
    {
        [Group]
        public class PlaceCommands
        {
            string typesStr { get; } = "";
            public PlaceCommands()
            {
                typesStr = $"`List of \"{FaultyBot.ModulePrefixes[typeof(Searches).Name]}place\" tags:`\n" + String.Join(", ", Enum.GetNames(typeof(PlaceType)));
            }

            public enum PlaceType
            {
                Cage, //http://www.placecage.com
                Steven, //http://www.stevensegallery.com
                Beard, //http://placebeard.it
                Fill, //http://www.fillmurray.com
                Bear, //https://www.placebear.com
                Kitten, //http://placekitten.com
                Bacon, //http://baconmockup.com
                Xoart, //http://xoart.link
            }

            [FaultyCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task Placelist(IUserMessage imsg)
            {
                var channel = (ITextChannel)imsg.Channel;

                await channel.SendMessageAsync(typesStr)
                             .ConfigureAwait(false);
            }

            [FaultyCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task Place(IUserMessage imsg, PlaceType placeType, uint width = 0, uint height = 0)
            {
                var channel = (ITextChannel)imsg.Channel;

                string url = "";
                switch (placeType)
                {
                    case PlaceType.Cage:
                        url = "http://www.placecage.com";
                        break;
                    case PlaceType.Steven:
                        url = "http://www.stevensegallery.com";
                        break;
                    case PlaceType.Beard:
                        url = "http://placebeard.it";
                        break;
                    case PlaceType.Fill:
                        url = "http://www.fillmurray.com";
                        break;
                    case PlaceType.Bear:
                        url = "https://www.placebear.com";
                        break;
                    case PlaceType.Kitten:
                        url = "http://placekitten.com";
                        break;
                    case PlaceType.Bacon:
                        url = "http://baconmockup.com";
                        break;
                    case PlaceType.Xoart:
                        url = "http://xoart.link";
                        break;
                }
                var rng = new FaultyRandom();
                if (width <= 0 || width > 1000)
                    width = (uint)rng.Next(250, 850);

                if (height <= 0 || height > 1000)
                    height = (uint)rng.Next(250, 850);

                url += $"/{width}/{height}";

                await channel.SendMessageAsync(url).ConfigureAwait(false);
            }
        }
    }
}

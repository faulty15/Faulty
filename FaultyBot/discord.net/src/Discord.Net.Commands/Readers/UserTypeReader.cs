﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    internal class UserTypeReader<T> : TypeReader
        where T : class, IUser
    {
        public override async Task<TypeReaderResult> Read(IUserMessage context, string input)
        {
            var results = new Dictionary<ulong, TypeReaderValue>();
            var guild = (context.Channel as IGuildChannel)?.Guild;
            IReadOnlyCollection<IUser> channelUsers = await context.Channel.GetUsersAsync().ConfigureAwait(false);
            IReadOnlyCollection<IGuildUser> guildUsers = null;
            ulong id;

            if (guild != null)
                guildUsers = await guild.GetUsersAsync().ConfigureAwait(false);

            //By Mention (1.0)
            if (MentionUtils.TryParseUser(input, out id))
            {
                if (guild != null)
                    AddResult(results, await guild.GetUserAsync(id).ConfigureAwait(false) as T, 1.00f);
                else
                    AddResult(results, await context.Channel.GetUserAsync(id).ConfigureAwait(false) as T, 1.00f);
            }

            //By Id (0.9)
            if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out id))
            {
                if (guild != null)
                    AddResult(results, await guild.GetUserAsync(id).ConfigureAwait(false) as T, 0.90f);
                else
                    AddResult(results, await context.Channel.GetUserAsync(id).ConfigureAwait(false) as T, 0.90f);
            }

            //By Username + Discriminator (0.7-0.85)
            int index = input.LastIndexOf('#');
            if (index >= 0)
            {
                string username = input.Substring(0, index);
                ushort discriminator;
                if (ushort.TryParse(input.Substring(index + 1), out discriminator))
                {
                    var channelUser = channelUsers.Where(x => x.DiscriminatorValue == discriminator &&
                        string.Equals(username, x.Username, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    AddResult(results, channelUser as T, channelUser.Username == username ? 0.85f : 0.75f);

                    var guildUser = channelUsers.Where(x => x.DiscriminatorValue == discriminator &&
                        string.Equals(username, x.Username, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    AddResult(results, guildUser as T, guildUser.Username == username ? 0.80f : 0.70f);
                }
            }

            //By Username (0.5-0.6)
            {
                foreach (var channelUser in channelUsers.Where(x => string.Equals(input, x.Username, StringComparison.OrdinalIgnoreCase)))
                    AddResult(results, channelUser as T, channelUser.Username == input ? 0.65f : 0.55f);

                foreach (var guildUser in guildUsers.Where(x => string.Equals(input, x.Username, StringComparison.OrdinalIgnoreCase)))
                    AddResult(results, guildUser as T, guildUser.Username == input ? 0.60f : 0.50f);
            }

            //By Nickname (0.5-0.6)
            {
                foreach (var channelUser in channelUsers.Where(x => string.Equals(input, (x as IGuildUser).Nickname, StringComparison.OrdinalIgnoreCase)))
                    AddResult(results, channelUser as T, (channelUser as IGuildUser).Nickname == input ? 0.65f : 0.55f);

                foreach (var guildUser in guildUsers.Where(x => string.Equals(input, (x as IGuildUser).Nickname, StringComparison.OrdinalIgnoreCase)))
                    AddResult(results, guildUser as T, (guildUser as IGuildUser).Nickname == input ? 0.60f : 0.50f);
            }

            if (results.Count > 0)
                return TypeReaderResult.FromSuccess(results.Values.ToArray());
            return TypeReaderResult.FromError(CommandError.ObjectNotFound, "User not found.");
        }

        private void AddResult(Dictionary<ulong, TypeReaderValue> results, T user, float score)
        {
            if (user != null && !results.ContainsKey(user.Id))
                results.Add(user.Id, new TypeReaderValue(user, score));
        }
    }
}

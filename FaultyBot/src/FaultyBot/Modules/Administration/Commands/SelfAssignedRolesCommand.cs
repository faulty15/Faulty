﻿using Discord;
using Discord.Commands;
using FaultyBot.Attributes;
using FaultyBot.Services;
using FaultyBot.Services.Database.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaultyBot.Modules.Administration
{
    public partial class Administration
    {
        [Group]
        public class SelfAssignedRolesCommands
        {
            
            [FaultyCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequirePermission(GuildPermission.ManageMessages)]
            public async Task AdSarm(IUserMessage imsg)
            {
                var channel = (ITextChannel)imsg.Channel;
                bool newval;
                using (var uow = DbHandler.UnitOfWork())
                {
                    var config = uow.GuildConfigs.For(channel.Guild.Id);
                    newval = config.AutoDeleteSelfAssignedRoleMessages = !config.AutoDeleteSelfAssignedRoleMessages;
                    await uow.CompleteAsync().ConfigureAwait(false);
                }

                await channel.SendMessageAsync($"ℹ️ Automatic deleting of `iam` and `iamn` confirmations has been {(newval ? "**enabled**" : "**disabled**")}.")
                             .ConfigureAwait(false);
            }

            [FaultyCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequirePermission(GuildPermission.ManageRoles)]
            public async Task Asar(IUserMessage umsg, [Remainder] IRole role)
            {
                var channel = (ITextChannel)umsg.Channel;

                IEnumerable<SelfAssignedRole> roles;

                string msg;
                using (var uow = DbHandler.UnitOfWork())
                {
                    roles = uow.SelfAssignedRoles.GetFromGuild(channel.Guild.Id);
                    if (roles.Any(s => s.RoleId == role.Id && s.GuildId == role.GuildId))
                    {
                        msg = $"💢 Role **{role.Name}** is already in the list.";
                    }
                    else
                    {
                        uow.SelfAssignedRoles.Add(new SelfAssignedRole {
                            RoleId = role.Id,
                            GuildId = role.GuildId
                        });
                        await uow.CompleteAsync();
                        msg = $"🆗 Role **{role.Name}** added to the list.";
                    }
                }
                await channel.SendMessageAsync(msg.ToString()).ConfigureAwait(false);
            }

            [FaultyCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequirePermission(GuildPermission.ManageRoles)]
            public async Task Rsar(IUserMessage umsg, [Remainder] IRole role)
            {
                var channel = (ITextChannel)umsg.Channel;

                bool success;
                using (var uow = DbHandler.UnitOfWork())
                {
                    success = uow.SelfAssignedRoles.DeleteByGuildAndRoleId(role.GuildId, role.Id);
                    await uow.CompleteAsync();
                }
                if (!success)
                {
                    await channel.SendMessageAsync("❎ That role is not self-assignable.").ConfigureAwait(false);
                    return;
                }
                await channel.SendMessageAsync($"🗑 **{role.Name}** has been removed from the list of self-assignable roles.").ConfigureAwait(false);
            }

            [FaultyCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task Lsar(IUserMessage umsg)
            {
                var channel = (ITextChannel)umsg.Channel;

                var toRemove = new ConcurrentHashSet<SelfAssignedRole>();
                var removeMsg = new StringBuilder();
                var msg = new StringBuilder();
                using (var uow = DbHandler.UnitOfWork())
                {
                    var roleModels = uow.SelfAssignedRoles.GetFromGuild(channel.Guild.Id);
                    msg.AppendLine($"ℹ️ There are `{roleModels.Count()}` self assignable roles:");
                    
                    foreach (var roleModel in roleModels)
                    {
                        var role = channel.Guild.Roles.FirstOrDefault(r => r.Id == roleModel.RoleId);
                        if (role == null)
                        {
                            uow.SelfAssignedRoles.Remove(roleModel);
                        }
                        else
                        {
                            msg.Append($"**{role.Name}**, ");
                        }
                    }
                    foreach (var role in toRemove)
                    {
                        removeMsg.AppendLine($"`{role.RoleId} not found. Cleaned up.`");
                    }
                    await uow.CompleteAsync();
                }
                await channel.SendMessageAsync(msg.ToString() + "\n\n" + removeMsg.ToString()).ConfigureAwait(false);
            }

            [FaultyCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            [RequirePermission(GuildPermission.ManageRoles)]
            public async Task Tesar(IUserMessage umsg)
            {
                var channel = (ITextChannel)umsg.Channel;

                bool areExclusive;
                using (var uow = DbHandler.UnitOfWork())
                {
                    var config = uow.GuildConfigs.For(channel.Guild.Id);

                    areExclusive = config.ExclusiveSelfAssignedRoles = !config.ExclusiveSelfAssignedRoles;
                    await uow.CompleteAsync();
                }
                string exl = areExclusive ? "**exclusive**." : "**not exclusive**.";
                await channel.SendMessageAsync("ℹ️ Self assigned roles are now " + exl);
            }

            [FaultyCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task Iam(IUserMessage umsg, [Remainder] IRole role)
            {
                var channel = (ITextChannel)umsg.Channel;
                var guildUser = (IGuildUser)umsg.Author;
                var usrMsg = (IUserMessage)umsg;

                GuildConfig conf;
                IEnumerable<SelfAssignedRole> roles;
                using (var uow = DbHandler.UnitOfWork())
                {
                    conf = uow.GuildConfigs.For(channel.Guild.Id);
                    roles = uow.SelfAssignedRoles.GetFromGuild(channel.Guild.Id);
                }
                SelfAssignedRole roleModel;
                if ((roleModel = roles.FirstOrDefault(r=>r.RoleId == role.Id)) == null)
                {
                    await channel.SendMessageAsync("💢 That role is not self-assignable.").ConfigureAwait(false);
                    return;
                }
                if (guildUser.Roles.Contains(role))
                {
                    await channel.SendMessageAsync($"❎ You already have **{role.Name}** role.").ConfigureAwait(false);
                    return;
                }

                if (conf.ExclusiveSelfAssignedRoles)
                {
                    var sameRoles = guildUser.Roles.Where(r => roles.Any(rm => rm.RoleId == r.Id));
                    if (sameRoles.Any())
                    {
                        await channel.SendMessageAsync($"❎ You already have **{sameRoles.FirstOrDefault().Name}** `exclusive self-assigned` role.").ConfigureAwait(false);
                        return;
                    }
                }
                try
                {
                    await guildUser.AddRolesAsync(role).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await channel.SendMessageAsync($"⚠️ I am unable to add that role to you. `I can't add roles to owners or other roles higher than my role in the role hierarchy.`").ConfigureAwait(false);
                    Console.WriteLine(ex);
                    return;
                }
                var msg = await channel.SendMessageAsync($"🆗 You now have **{role.Name}** role.").ConfigureAwait(false);

                if (conf.AutoDeleteSelfAssignedRoleMessages)
                {
                    var t = Task.Run(async () =>
                    {
                        await Task.Delay(3000).ConfigureAwait(false);
                        try { await msg.DeleteAsync().ConfigureAwait(false); } catch { } // if 502 or something, i don't want bot crashing
                        try { await usrMsg.DeleteAsync().ConfigureAwait(false); } catch { }
                    });
                }
            }

            [FaultyCommand, Usage, Description, Aliases]
            [RequireContext(ContextType.Guild)]
            public async Task Iamnot(IUserMessage umsg, [Remainder] IRole role)
            {
                var channel = (ITextChannel)umsg.Channel;
                var guildUser = (IGuildUser)umsg.Author;

                GuildConfig conf;
                IEnumerable<SelfAssignedRole> roles;
                using (var uow = DbHandler.UnitOfWork())
                {
                    conf = uow.GuildConfigs.For(channel.Guild.Id);
                    roles = uow.SelfAssignedRoles.GetFromGuild(channel.Guild.Id);
                }
                SelfAssignedRole roleModel;
                if ((roleModel = roles.FirstOrDefault(r => r.RoleId == role.Id)) == null)
                {
                    await channel.SendMessageAsync("💢 That role is not self-assignable.").ConfigureAwait(false);
                    return;
                }
                if (!guildUser.Roles.Contains(role))
                {
                    await channel.SendMessageAsync($"❎ You don't have **{role.Name}** role.").ConfigureAwait(false);
                    return;
                }
                try
                {
                    await guildUser.RemoveRolesAsync(role).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    await channel.SendMessageAsync($"⚠️ I am unable to add that role to you. `I can't remove roles to owners or other roles higher than my role in the role hierarchy.`").ConfigureAwait(false);
                    return;
                }
                var msg = await channel.SendMessageAsync($"🆗 You no longer have **{role.Name}** role.").ConfigureAwait(false);

                if (conf.AutoDeleteSelfAssignedRoleMessages)
                {
                    var t = Task.Run(async () =>
                    {
                        await Task.Delay(3000).ConfigureAwait(false);
                        try { await msg.DeleteAsync().ConfigureAwait(false); } catch { } // if 502 or something, i don't want bot crashing
                        try { await umsg.DeleteAsync().ConfigureAwait(false); } catch { }
                    });
                }
            }
        }
    }
}

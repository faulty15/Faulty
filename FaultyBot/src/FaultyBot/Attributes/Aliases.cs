﻿using Discord.Commands;
using FaultyBot.Services;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FaultyBot.Attributes
{
    public class Aliases : AliasAttribute
    {
        public Aliases([CallerMemberName] string memberName = "") : base(Localization.LoadCommandString(memberName.ToLowerInvariant() + "_cmd").Split(' ').Skip(1).ToArray())
        {
        }
    }
}

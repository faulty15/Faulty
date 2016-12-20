using Discord.Commands;
using FaultyBot.Services;
using System.Runtime.CompilerServices;

namespace FaultyBot.Attributes
{
    public class Description : SummaryAttribute
    {
        public Description([CallerMemberName] string memberName="") : base(Localization.LoadCommandString(memberName.ToLowerInvariant() + "_desc"))
        {

        }
    }
}

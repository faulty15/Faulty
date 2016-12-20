using Discord.Commands;
using FaultyBot.Services;
using System.Runtime.CompilerServices;

namespace FaultyBot.Attributes
{
    public class Usage : RemarksAttribute
    {
        public Usage([CallerMemberName] string memberName="") : base(Localization.LoadCommandString(memberName.ToLowerInvariant()+"_usage"))
        {

        }
    }
}

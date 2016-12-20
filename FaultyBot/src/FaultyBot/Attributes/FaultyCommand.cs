using Discord.Commands;
using FaultyBot.Services;
using System.Runtime.CompilerServices;

namespace FaultyBot.Attributes
{
    public class FaultyCommand : CommandAttribute
    {
        public FaultyCommand([CallerMemberName] string memberName="") : base(Localization.LoadCommandString(memberName.ToLowerInvariant() + "_cmd").Split(' ')[0])
        {

        }
    }
}

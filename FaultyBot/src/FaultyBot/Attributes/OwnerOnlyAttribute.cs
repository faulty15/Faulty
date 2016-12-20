using System.Threading.Tasks;
using Discord.Commands;
using Discord;

namespace FaultyBot.Attributes
{
    public class OwnerOnlyAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(IUserMessage context, Command executingCommand, object moduleInstance) =>
            Task.FromResult((FaultyBot.Credentials.IsOwner(context.Author) ? PreconditionResult.FromSuccess() : PreconditionResult.FromError("Not owner")));
    }
}
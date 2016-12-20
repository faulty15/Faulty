using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace FaultyBot.TypeReaders
{
    public class ModuleTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(IUserMessage context, string input)
        {
            input = input.ToUpperInvariant();
            var module = FaultyBot.CommandService.Modules.FirstOrDefault(m => m.Name.ToUpperInvariant() == input);
            if (module == null)
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "No such module found."));

            return Task.FromResult(TypeReaderResult.FromSuccess(module));
        }
    }
}

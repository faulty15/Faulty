using System.Collections.Generic;

namespace FaultyBot.Services.Database.Models
{
    public class BotConfig : DbEntity
    {
        public HashSet<BlacklistItem> Blacklist { get; set; }
        public ulong BufferSize { get; set; } = 4000000;
        public bool ForwardMessages { get; set; } = true;
        public bool ForwardToAllOwners { get; set; } = true;

        public float CurrencyGenerationChance { get; set; } = 0.02f;
        public int CurrencyGenerationCooldown { get; set; } = 10;

        public HashSet<ModulePrefix> ModulePrefixes { get; set; } = new HashSet<ModulePrefix>();

        public List<PlayingStatus> RotatingStatusMessages { get; set; } = new List<PlayingStatus>();

        public bool RotatingStatuses { get; set; } = false;
        public string RemindMessageFormat { get; set; } = "❗⏰**I've been told to remind you to '%message%' now by %user%.**⏰❗";


        public string CurrencySign { get; set; } = "🌸";
        public string CurrencyName { get; set; } = "Faulty Flower";
        public string CurrencyPluralName { get; set; } = "Faulty Flowers";

        public HashSet<EightBallResponse> EightBallResponses { get; set; } = new HashSet<EightBallResponse>();
        public HashSet<RaceAnimal> RaceAnimals { get; set; } = new HashSet<RaceAnimal>();
;

        public int MigrationVersion { get; set; }
    }

    public class PlayingStatus :DbEntity
    {
        public string Status { get; set; }
    }

    public class BlacklistItem : DbEntity
    {
        public ulong ItemId { get; set; }
        public BlacklistType Type { get; set; }

        public enum BlacklistType
        {
            Server,
            Channel,
            User
        }
    }

    public class EightBallResponse : DbEntity
    {
        public string Text { get; set; }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is EightBallResponse))
                return base.Equals(obj);

            return ((EightBallResponse)obj).Text == Text;
        }
    }

    public class RaceAnimal : DbEntity
    {
        public string Icon { get; set; }
        public string Name { get; set; }

        public override int GetHashCode()
        {
            return Icon.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RaceAnimal))
                return base.Equals(obj);

            return ((RaceAnimal)obj).Icon == Icon;
        }
    }
    
    public class ModulePrefix : DbEntity
    {
        public string ModuleName { get; set; }
        public string Prefix { get; set; }

        public override int GetHashCode()
        {
            return ModuleName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(!(obj is ModulePrefix))
                return base.Equals(obj);

            return ((ModulePrefix)obj).ModuleName == ModuleName;
        }
    }
}

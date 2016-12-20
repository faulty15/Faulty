using Microsoft.EntityFrameworkCore;
using FaultyBot.Services.Database;

namespace FaultyBot.Services
{
    public class DbHandler
    {
        private static DbHandler _instance = null;
        public static DbHandler Instance = _instance ?? (_instance = new DbHandler());
        private readonly DbContextOptions options;

        private string connectionString { get; }

        static DbHandler() { }

        private DbHandler() {
            connectionString = FaultyBot.Credentials.Db.ConnectionString;
            var optionsBuilder = new DbContextOptionsBuilder();
            optionsBuilder.UseSqlite(FaultyBot.Credentials.Db.ConnectionString);
            options = optionsBuilder.Options;
            //switch (FaultyBot.Credentials.Db.Type.ToUpperInvariant())
            //{
            //    case "SQLITE":
            //        dbType = typeof(FaultySqliteContext);
            //        break;
            //    //case "SQLSERVER":
            //    //    dbType = typeof(FaultySqlServerContext);
            //    //    break;
            //    default:
            //        break;

            //}
        }

        public FaultyContext GetDbContext() =>
            new FaultyContext(options);

        public IUnitOfWork GetUnitOfWork() =>
            new UnitOfWork(GetDbContext());

        public static IUnitOfWork UnitOfWork() =>
            DbHandler.Instance.GetUnitOfWork();
    }
}

using Npgsql;
using Voartec.Config;

namespace Voartec.Helpers
{
    public class Database
    {
        private static Database db = null;
        private NpgsqlConnection conn;

        public Database()
        {
            var configuration = Builder.GetConfiguration();
            conn = new NpgsqlConnection(configuration.GetSection("DB:ConnectionString").Value);
        }

        public static Database GetState()
        {
            if (db == null)
            {
                db = new Database();
            }

            return db;
        }

        public NpgsqlConnection GetCon()
        {
            return conn;
        }

        public void Close()
        {
            db = null;
        }
    }
}
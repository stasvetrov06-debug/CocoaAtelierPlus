using Microsoft.Data.Sqlite;

namespace CocoaAtelierPlus.Services
{
    public static class Database
    {
        private static readonly string DbPath = Path.Combine(AppContext.BaseDirectory, "CocoaAtelier.db");

        public static string ConnectionString => $"Data Source={DbPath};Foreign Keys=True";

        public static void Init()
        {
            // Если файла нет или он поврежден, база создается заново из скрипта.
            if (HasSchema())
            {
                return;
            }

            SqliteConnection.ClearAllPools();

            if (File.Exists(DbPath))
            {
                File.Delete(DbPath);
            }

            var scriptPath = Path.Combine(AppContext.BaseDirectory, "CocoaAtelierDB.sql");
            var script = File.ReadAllText(scriptPath);

            try
            {
                using (var connection = new SqliteConnection(ConnectionString))
                {
                    connection.Open();

                    using var command = connection.CreateCommand();
                    command.CommandText = script;
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                SqliteConnection.ClearAllPools();

                if (File.Exists(DbPath))
                {
                    File.Delete(DbPath);
                }

                throw;
            }
        }

        public static SqliteConnection OpenConnection()
        {
            var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        private static bool HasSchema()
        {
            if (!File.Exists(DbPath))
            {
                return false;
            }

            if (new FileInfo(DbPath).Length == 0)
            {
                return false;
            }

            try
            {
                using var connection = new SqliteConnection(ConnectionString + ";Pooling=False");
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'material'";
                return Convert.ToInt32(command.ExecuteScalar()) > 0;
            }
            catch (SqliteException)
            {
                return false;
            }
        }
    }
}

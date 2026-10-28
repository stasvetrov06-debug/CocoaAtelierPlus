using CocoaAtelierPlus.Models;
using Microsoft.Data.Sqlite;

namespace CocoaAtelierPlus.Services
{
    public static class SupplierService
    {
        public static List<SupplierForm> GetAll()
        {
            var result = new List<SupplierForm>();

            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT supplier_id, supplier_type, name, inn, rating, start_date
                FROM supplier
                ORDER BY name";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new SupplierForm
                {
                    SupplierId = reader.GetInt32(0),
                    SupplierType = reader.GetString(1),
                    Name = reader.GetString(2),
                    Inn = reader.GetString(3),
                    Rating = reader.GetInt32(4),
                    StartDate = reader.GetString(5)
                });
            }

            return result;
        }

        public static SupplierForm? GetById(int supplierId)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT supplier_id, supplier_type, name, inn, rating, start_date
                FROM supplier
                WHERE supplier_id = @id";
            command.Parameters.AddWithValue("@id", supplierId);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new SupplierForm
            {
                SupplierId = reader.GetInt32(0),
                SupplierType = reader.GetString(1),
                Name = reader.GetString(2),
                Inn = reader.GetString(3),
                Rating = reader.GetInt32(4),
                StartDate = reader.GetString(5)
            };
        }

        public static void Add(SupplierForm form)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO supplier (supplier_type, name, inn, rating, start_date)
                VALUES (@type, @name, @inn, @rating, @date)";
            command.Parameters.AddWithValue("@type", form.SupplierType);
            command.Parameters.AddWithValue("@name", form.Name);
            command.Parameters.AddWithValue("@inn", form.Inn);
            command.Parameters.AddWithValue("@rating", form.Rating ?? 0);
            command.Parameters.AddWithValue("@date", form.StartDate);
            command.ExecuteNonQuery();
        }

        public static void Update(SupplierForm form)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE supplier
                SET supplier_type = @type,
                    name = @name,
                    inn = @inn,
                    rating = @rating,
                    start_date = @date
                WHERE supplier_id = @id";
            command.Parameters.AddWithValue("@id", form.SupplierId ?? 0);
            command.Parameters.AddWithValue("@type", form.SupplierType);
            command.Parameters.AddWithValue("@name", form.Name);
            command.Parameters.AddWithValue("@inn", form.Inn);
            command.Parameters.AddWithValue("@rating", form.Rating ?? 0);
            command.Parameters.AddWithValue("@date", form.StartDate);
            command.ExecuteNonQuery();
        }

        public static void Delete(int supplierId)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM supplier WHERE supplier_id = @id";
            command.Parameters.AddWithValue("@id", supplierId);
            command.ExecuteNonQuery();
        }

        public static bool NameExists(string name, int? supplierId)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM supplier WHERE name = @name AND supplier_id <> @id";
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@id", supplierId ?? 0);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        public static bool InnExists(string inn, int? supplierId)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM supplier WHERE inn = @inn AND supplier_id <> @id";
            command.Parameters.AddWithValue("@inn", inn);
            command.Parameters.AddWithValue("@id", supplierId ?? 0);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }
    }
}

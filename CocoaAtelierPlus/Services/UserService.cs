using CocoaAtelierPlus.Models;
using Microsoft.Data.Sqlite;

namespace CocoaAtelierPlus.Services
{
    public static class UserService
    {
        public static AppUser? Authenticate(string login, string password)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT u.user_id, u.login, u.full_name, u.email, u.password, u.role_id, r.name, u.is_active
                FROM app_user u
                JOIN role r ON r.role_id = u.role_id
                WHERE u.login = @login AND u.password = @password";
            command.Parameters.AddWithValue("@login", login);
            command.Parameters.AddWithValue("@password", password);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new AppUser
            {
                UserId = reader.GetInt32(0),
                Login = reader.GetString(1),
                FullName = reader.GetString(2),
                Email = reader.GetString(3),
                Password = reader.GetString(4),
                RoleId = reader.GetInt32(5),
                RoleName = reader.GetString(6),
                IsActive = reader.GetInt32(7) == 1
            };
        }

        public static List<AppUser> GetAll()
        {
            var result = new List<AppUser>();

            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT u.user_id, u.login, u.full_name, u.email, u.password, u.role_id, r.name, u.is_active
                FROM app_user u
                JOIN role r ON r.role_id = u.role_id
                ORDER BY u.login";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(ReadUser(reader));
            }

            return result;
        }

        public static AppUser? GetById(int userId)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT u.user_id, u.login, u.full_name, u.email, u.password, u.role_id, r.name, u.is_active
                FROM app_user u
                JOIN role r ON r.role_id = u.role_id
                WHERE u.user_id = @id";
            command.Parameters.AddWithValue("@id", userId);

            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return ReadUser(reader);
        }

        public static void Add(UserForm form)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO app_user (login, full_name, email, password, role_id, is_active)
                VALUES (@login, @name, @email, @password, @role, @active)";
            command.Parameters.AddWithValue("@login", form.Login);
            command.Parameters.AddWithValue("@name", form.FullName);
            command.Parameters.AddWithValue("@email", form.Email);
            command.Parameters.AddWithValue("@password", form.Password);
            command.Parameters.AddWithValue("@role", form.RoleId ?? 0);
            command.Parameters.AddWithValue("@active", form.IsActive ? 1 : 0);
            command.ExecuteNonQuery();
        }

        public static void Update(UserForm form)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE app_user
                SET login = @login,
                    full_name = @name,
                    email = @email,
                    password = @password,
                    role_id = @role,
                    is_active = @active
                WHERE user_id = @id";
            command.Parameters.AddWithValue("@id", form.UserId ?? 0);
            command.Parameters.AddWithValue("@login", form.Login);
            command.Parameters.AddWithValue("@name", form.FullName);
            command.Parameters.AddWithValue("@email", form.Email);
            command.Parameters.AddWithValue("@password", form.Password);
            command.Parameters.AddWithValue("@role", form.RoleId ?? 0);
            command.Parameters.AddWithValue("@active", form.IsActive ? 1 : 0);
            command.ExecuteNonQuery();
        }

        public static List<RoleItem> GetRoles()
        {
            var result = new List<RoleItem>();

            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT role_id, name FROM role ORDER BY role_id";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new RoleItem
                {
                    RoleId = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }

            return result;
        }

        public static bool LoginExists(string login, int? userId)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM app_user WHERE login = @login AND user_id <> @id";
            command.Parameters.AddWithValue("@login", login);
            command.Parameters.AddWithValue("@id", userId ?? 0);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        public static bool EmailExists(string email, int? userId)
        {
            using var connection = Database.OpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM app_user WHERE email = @email AND user_id <> @id";
            command.Parameters.AddWithValue("@email", email);
            command.Parameters.AddWithValue("@id", userId ?? 0);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        private static AppUser ReadUser(SqliteDataReader reader)
        {
            return new AppUser
            {
                UserId = reader.GetInt32(0),
                Login = reader.GetString(1),
                FullName = reader.GetString(2),
                Email = reader.GetString(3),
                Password = reader.GetString(4),
                RoleId = reader.GetInt32(5),
                RoleName = reader.GetString(6),
                IsActive = reader.GetInt32(7) == 1
            };
        }
    }
}

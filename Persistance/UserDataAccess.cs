using Npgsql;

namespace robot_controller_api.Persistence
{
    public static class UserDataAccess
    {
        private static string connectionString = "Host=localhost;Username=postgres;Password=password;Database=sit331";

        public static List<UserModel> GetUsers()
        {
            var users = new List<UserModel>();
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"SELECT id, email, firstname, lastname, passwordhash, createddate, modifieddate, description, role FROM public.""user""", conn);
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                var user = new UserModel(
                    (int)dr["id"],
                    (string)dr["email"],
                    (string)dr["firstname"],
                    (string)dr["lastname"],
                    (string)dr["passwordhash"],
                    (DateTime)dr["createddate"],
                    (DateTime)dr["modifieddate"],
                    dr["description"] as string,
                    dr["role"] as string
                );
                users.Add(user);
            }

            return users;
        }

        public static int AddUser(UserModel user)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
                INSERT INTO public.""user"" (email, firstname, lastname, passwordhash, createddate, modifieddate, description, role)
                VALUES (@Email, @FirstName, @LastName, @PasswordHash, @CreatedDate, @ModifiedDate, @Description, @Role)
                RETURNING id", conn);

            cmd.Parameters.AddWithValue("Email", user.Email);
            cmd.Parameters.AddWithValue("FirstName", user.FirstName);
            cmd.Parameters.AddWithValue("LastName", user.LastName);
            cmd.Parameters.AddWithValue("PasswordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("CreatedDate", user.CreatedDate);
            cmd.Parameters.AddWithValue("ModifiedDate", user.ModifiedDate);
            cmd.Parameters.AddWithValue("Description", (object?)user.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("Role", (object?)user.Role ?? DBNull.Value);

            return (int)cmd.ExecuteScalar();
        }

        public static void UpdateUser(int id, UserModel updatedUser)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"
                UPDATE public.""user""
                SET email = @Email,
                    firstname = @FirstName,
                    lastname = @LastName,
                    passwordhash = @PasswordHash,
                    createddate = @CreatedDate,
                    modifieddate = @ModifiedDate,
                    description = @Description,
                    role = @Role
                WHERE id = @Id", conn);

            cmd.Parameters.AddWithValue("Id", id);
            cmd.Parameters.AddWithValue("Email", updatedUser.Email);
            cmd.Parameters.AddWithValue("FirstName", updatedUser.FirstName);
            cmd.Parameters.AddWithValue("LastName", updatedUser.LastName);
            cmd.Parameters.AddWithValue("PasswordHash", updatedUser.PasswordHash);
            cmd.Parameters.AddWithValue("CreatedDate", updatedUser.CreatedDate);
            cmd.Parameters.AddWithValue("ModifiedDate", updatedUser.ModifiedDate);
            cmd.Parameters.AddWithValue("Description", (object?)updatedUser.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("Role", (object?)updatedUser.Role ?? DBNull.Value);

            cmd.ExecuteNonQuery();
        }

        public static void DeleteUser(int id)
        {
            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            using var cmd = new NpgsqlCommand(@"DELETE FROM public.""user"" WHERE id = @Id", conn);
            cmd.Parameters.AddWithValue("Id", id);

            cmd.ExecuteNonQuery();
        }
    }
}

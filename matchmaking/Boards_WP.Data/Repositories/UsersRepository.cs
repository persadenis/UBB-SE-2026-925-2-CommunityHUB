using System;
using System.Data;

using Microsoft.Data.SqlClient;

using Boards_WP.Data.Models;
using Boards_WP.Data.Repositories.Interfaces;

namespace Boards_WP.Data.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;

        public UsersRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User GetUserByID(int userID)
        {
            const string query = @"
                SELECT 
                    userID, 
                    username, 
                    email, 
                    passwordHash,
                    avatarUrl, 
                    bio, 
                    status
                FROM Users
                WHERE userID = @UserID";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.Add("@UserID", SqlDbType.Int).Value = userID;

            connection.Open();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapUser(reader);
            }

            return null;
        }

        public User GetUserByEmail(string email)
        {
            const string query = @"
                SELECT 
                    userID, 
                    username, 
                    email, 
                    passwordHash,
                    avatarUrl, 
                    bio, 
                    status
                FROM Users
                WHERE email = @Email";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = email;

            connection.Open();
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return MapUser(reader);
            }

            return null;
        }

        private static User MapUser(SqlDataReader reader)
        {
            return new User
            {
                UserID = reader.GetInt32(reader.GetOrdinal("userID")),
                Username = reader.GetString(reader.GetOrdinal("username")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                PasswordHash = reader.IsDBNull(reader.GetOrdinal("passwordHash")) ? null : reader.GetString(reader.GetOrdinal("passwordHash")),
                AvatarUrl = reader.IsDBNull(reader.GetOrdinal("avatarUrl")) ? null : reader.GetString(reader.GetOrdinal("avatarUrl")),
                Bio = reader.IsDBNull(reader.GetOrdinal("bio")) ? null : reader.GetString(reader.GetOrdinal("bio")),
                Status = reader.IsDBNull(reader.GetOrdinal("status")) ? null : reader.GetString(reader.GetOrdinal("status"))
            };
        }
    }
}
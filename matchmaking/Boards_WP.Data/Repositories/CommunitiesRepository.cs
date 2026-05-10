using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using System.Xml.Linq;

namespace Boards_WP.Data.Repositories;

public class CommunitiesRepository : ICommunitiesRepository
{

    private readonly String _connectionString;

    public CommunitiesRepository(String connectionString)
    {
        _connectionString = connectionString;
    }
    public int AddCommunity(Community NewCommunity)
    {
        const string query = @"
                INSERT INTO Communities (name, description, picture, banner, membersNumber, adminID)
                OUTPUT INSERTED.communityID
                VALUES (@name, @description, @picture, @banner, @membersNumber, @adminID)";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@name", SqlDbType.NVarChar).Value = NewCommunity.Name;
        command.Parameters.Add("@description", SqlDbType.NVarChar).Value = NewCommunity.Description;
        command.Parameters.Add("@picture", SqlDbType.VarBinary).Value = NewCommunity.Picture ?? (object)DBNull.Value;
        command.Parameters.Add("@banner", SqlDbType.VarBinary).Value = NewCommunity.Banner ?? (object)DBNull.Value;
        command.Parameters.Add("@membersNumber", SqlDbType.Int).Value = NewCommunity.MembersNumber;
        command.Parameters.Add("@adminID", SqlDbType.Int).Value = NewCommunity.Admin.UserID;

        connection.Open();
        return (int)command.ExecuteScalar()!;
    }

    public void AddUserToCommunity(int CommunityID, int UserID)
    {
        const string query = @"
        INSERT INTO CommunitiesUsers (communityID, userID)
        VALUES (@communityID, @userID)";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@communityID", SqlDbType.Int).Value = CommunityID;
        command.Parameters.Add("@userID", SqlDbType.Int).Value = UserID;

        connection.Open();
        command.ExecuteNonQuery();
    }

    public bool CheckOwner(int CommunityID, int OwnerID)
    {
        const string query = @"SELECT * FROM Communities WHERE communityID = @communityID";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@communityID", SqlDbType.Int).Value = CommunityID;

        connection.Open();

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            int ownerIDFromDB = (int)reader["adminID"];

            return ownerIDFromDB == OwnerID;
        }

        return false;
    }

    public void DecreaseMembersNumber(int CommunityID)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        int membersNumber = 0;

        string selectQuery = @"SELECT membersNumber FROM Communities WHERE communityID = @communityID";

        using (var selectCommand = new SqlCommand(selectQuery, connection))
        {
            selectCommand.Parameters.Add("@communityID", SqlDbType.Int).Value = CommunityID;

            using var reader = selectCommand.ExecuteReader();

            if (reader.Read())
            {
                membersNumber = (int)reader["membersNumber"];
            }
        }
        membersNumber--;

        string updateQuery = @"UPDATE Communities 
                           SET membersNumber = @membersNumber 
                           WHERE communityID = @communityID";

        using (var updateCommand = new SqlCommand(updateQuery, connection))
        {
            updateCommand.Parameters.Add("@membersNumber", SqlDbType.Int).Value = membersNumber;
            updateCommand.Parameters.Add("@communityID", SqlDbType.Int).Value = CommunityID;

            updateCommand.ExecuteNonQuery();
        }
    }

    public List<Community> GetCommunitiesByNamesMatch(string Name)
    {
        var communities = new List<Community>();

        const string query = @"
SELECT
    c.communityID,
    c.name,
    c.description,
    c.picture,
    c.banner,
    c.membersNumber,

    u.userID AS adminUserID,
    u.username AS adminUsername,
    u.email AS adminEmail,
    u.passwordHash AS adminPasswordHash,
    u.avatarUrl AS adminAvatarUrl,
    u.bio AS adminBio,
    u.status AS adminStatus

    FROM Communities c
    INNER JOIN Users u ON c.adminID = u.userID
    WHERE c.name LIKE @name";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@name", SqlDbType.NVarChar).Value = $"%{Name}%";

        connection.Open();

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var admin = new User
            {
                UserID = (int)reader["adminUserID"],
                Username = reader["adminUsername"].ToString(),
                Email = reader["adminEmail"].ToString(),
                PasswordHash = reader["adminPasswordHash"].ToString(),
                AvatarUrl = reader["adminAvatarUrl"].ToString(),
                Bio = reader["adminBio"].ToString(),
                Status = reader["adminStatus"].ToString()
            };

            var community = new Community
            {
                CommunityID = (int)reader["communityID"],
                Name = reader["name"].ToString(),
                Description = reader["description"].ToString(),
                Picture = reader["picture"] as byte[],
                Banner = reader["banner"] as byte[],
                MembersNumber = (int)reader["membersNumber"],
                Admin = admin
            };

            communities.Add(community);
        }

        return communities;
    }

    public List<Community> GetCommunitiesUserIsPartOf(int UserID)
    {
        var communities = new List<Community>();
        const string query = @"
        SELECT
   
        c.communityID,
        c.name,
        c.description,
        c.picture,
        c.banner,
        c.membersNumber,

        u.userID ,
        u.username, 
        u.email ,
        u.passwordHash ,
        u.avatarUrl,
        u.bio,
        u.status

        FROM CommunitiesUsers cu
        INNER JOIN Communities c ON cu.communityID = c.communityID
        INNER JOIN Users u ON c.adminID = u.userID
        WHERE cu.userID = @userID";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@userID", SqlDbType.Int).Value = UserID;

        connection.Open();

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var admin = new User
            {
                UserID = (int)reader["userID"],
                Username = reader["username"].ToString(),
                Email = reader["email"].ToString(),
                PasswordHash = reader["passwordHash"].ToString(),
                AvatarUrl = reader["avatarUrl"].ToString(),
                Bio = reader["bio"].ToString(),
                Status = reader["status"].ToString()
            };

            var community = new Community
            {
                CommunityID = (int)reader["communityID"],
                Name = reader["name"].ToString(),
                Description = reader["description"].ToString(),
                Picture = reader["picture"] as byte[],
                Banner = reader["banner"] as byte[],
                MembersNumber = (int)reader["membersNumber"],
                Admin = admin
            };

            communities.Add(community);
        }

        return communities;
    }

    public void IncreaseMembersNumber(int CommunityID)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        int membersNumber = 0;

        string selectQuery = @"SELECT membersNumber FROM Communities WHERE communityID = @communityID";

        using (var selectCommand = new SqlCommand(selectQuery, connection))
        {
            selectCommand.Parameters.Add("@communityID", SqlDbType.Int).Value = CommunityID;

            using var reader = selectCommand.ExecuteReader();

            if (reader.Read())
            {
                membersNumber = (int)reader["membersNumber"];
            }
        }
        membersNumber++;

        string updateQuery = @"UPDATE Communities 
                           SET membersNumber = @membersNumber 
                           WHERE communityID = @communityID";

        using (var updateCommand = new SqlCommand(updateQuery, connection))
        {
            updateCommand.Parameters.Add("@membersNumber", SqlDbType.Int).Value = membersNumber;
            updateCommand.Parameters.Add("@communityID", SqlDbType.Int).Value = CommunityID;

            updateCommand.ExecuteNonQuery();
        }
    }

    public bool IsPartOfCommunity(int UserID, int CommunityID)
    {
        const string query = @"SELECT COUNT(*) FROM CommunitiesUsers WHERE userID = @userID AND communityID = @communityID";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);

        connection.Open();

        command.Parameters.Add("@userID", SqlDbType.Int).Value = UserID;
        command.Parameters.Add("@communityID", SqlDbType.Int).Value = CommunityID;

        int found = (int)command.ExecuteScalar()!;

        return found != 0;

    }

    public void RemoveUserFromCommunity(int CommunityID, int UserID)
    {
        const string query = @"
        DELETE FROM CommunitiesUsers
        WHERE communityID = @communityID AND userID = @userID";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@communityID", SqlDbType.Int).Value = CommunityID;
        command.Parameters.Add("@userID", SqlDbType.Int).Value = UserID;

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void UpdateBanner(int CommunityID, byte[] NewBanner)
    {
        const string query = @"
        UPDATE Communities
        SET banner = @banner
        WHERE communityID = @communityID";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@banner", SqlDbType.VarBinary).Value = NewBanner;
        command.Parameters.Add("@communityID", SqlDbType.Int).Value = CommunityID;

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void UpdateCommunityPicture(int CommunityID, byte[] NewPhoto)
    {
        const string query = @"
        UPDATE Communities
        SET picture = @picture
        WHERE communityID = @communityID";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@picture", SqlDbType.VarBinary).Value = NewPhoto;
        command.Parameters.Add("@communityID", SqlDbType.Int).Value = CommunityID;

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void UpdateDescription(int CommunityID, string NewDescription)
    {
        const string query = @"
        UPDATE Communities
        SET description = @description
        WHERE communityID = @communityID";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@description", SqlDbType.NVarChar).Value = NewDescription;
        command.Parameters.Add("@communityID", SqlDbType.Int).Value = CommunityID;

        connection.Open();
        command.ExecuteNonQuery();
    }

    public Community? GetCommunityByID(int communityID)
    {
        const string query = @"
    SELECT
        c.communityID,
        c.name,
        c.description,
        c.picture,
        c.banner,
        c.membersNumber,

        u.userID AS adminUserID,
        u.username AS adminUsername,
        u.email AS adminEmail,
        u.passwordHash AS adminPasswordHash,
        u.avatarUrl AS adminAvatarUrl,
        u.bio AS adminBio,
        u.status AS adminStatus

    FROM Communities c
    INNER JOIN Users u ON c.adminID = u.userID
    WHERE c.communityID = @communityID";

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);

        command.Parameters.Add("@communityID", SqlDbType.Int).Value = communityID;

        connection.Open();

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            var admin = new User
            {
                UserID = (int)reader["adminUserID"],
                Username = reader["adminUsername"].ToString(),
                Email = reader["adminEmail"].ToString(),
                PasswordHash = reader["adminPasswordHash"].ToString(),
                AvatarUrl = reader["adminAvatarUrl"].ToString(),
                Bio = reader["adminBio"].ToString(),
                Status = reader["adminStatus"].ToString()
            };

            return new Community
            {
                CommunityID = (int)reader["communityID"],
                Name = reader["name"].ToString(),
                Description = reader["description"].ToString(),
                Picture = reader["picture"] as byte[],
                Banner = reader["banner"] as byte[],
                MembersNumber = (int)reader["membersNumber"],
                Admin = admin
            };
        }

        return null;
    }
}

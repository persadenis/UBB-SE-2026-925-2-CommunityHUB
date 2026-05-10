using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<ConnectionStrings>();
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<CommunityRepository>();
builder.Services.AddScoped<TinderRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "ok",
    application = "Matchmaking.Api",
    timestamp = DateTimeOffset.UtcNow
}));

app.MapPost("/api/auth/login", async (LoginRequest request, AuthRepository repository) =>
{
    if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
    {
        return Results.BadRequest(new { error = "Email and password are required." });
    }

    UserDto? user = await repository.LoginAsync(request.Email.Trim(), request.Password);
    return user is null
        ? Results.Unauthorized()
        : Results.Ok(new LoginResponse(user.UserId, user.Username, user.Email));
});

app.MapGet("/api/communities/users/{userId:int}", async (int userId, CommunityRepository repository) =>
{
    IReadOnlyList<CommunityDto> communities = await repository.GetCommunitiesForUserAsync(userId);
    return Results.Ok(communities);
});

app.MapGet("/api/tinder/profile/{userId:int}", async (int userId, TinderRepository repository) =>
{
    DatingProfileDto? profile = await repository.GetProfileAsync(userId);
    return profile is null ? Results.NotFound() : Results.Ok(profile);
});

app.MapGet("/api/tinder/discover/{userId:int}", async (int userId, TinderRepository repository) =>
{
    IReadOnlyList<DatingProfileDto> profiles = await repository.GetDiscoverCandidatesAsync(userId);
    return Results.Ok(profiles);
});

app.Run();

internal sealed class ConnectionStrings(IConfiguration configuration)
{
    public string CommunityConnection =>
        configuration.GetConnectionString("CommunityConnection")
        ?? throw new InvalidOperationException("Missing CommunityConnection connection string.");

    public string DefaultConnection =>
        configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing DefaultConnection connection string.");
}

internal sealed class AuthRepository(ConnectionStrings connectionStrings)
{
    public async Task<UserDto?> LoginAsync(string email, string password)
    {
        const string query = """
            SELECT userID, username, email, passwordHash
            FROM Users
            WHERE email = @email;
            """;

        await using SqlConnection connection = new(connectionStrings.CommunityConnection);
        await using SqlCommand command = new(query, connection);
        command.Parameters.Add("@email", SqlDbType.NVarChar, 255).Value = email;

        await connection.OpenAsync();
        await using SqlDataReader reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        string storedHash = reader.GetString(reader.GetOrdinal("passwordHash"));
        if (!string.Equals(storedHash, HashPassword(password), StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return new UserDto(
            reader.GetInt32(reader.GetOrdinal("userID")),
            reader.GetString(reader.GetOrdinal("username")),
            reader.GetString(reader.GetOrdinal("email")));
    }

    private static string HashPassword(string password)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        StringBuilder builder = new(bytes.Length * 2);
        foreach (byte value in bytes)
        {
            builder.Append(value.ToString("x2"));
        }

        return builder.ToString();
    }
}

internal sealed class CommunityRepository(ConnectionStrings connectionStrings)
{
    public async Task<IReadOnlyList<CommunityDto>> GetCommunitiesForUserAsync(int userId)
    {
        const string query = """
            SELECT c.communityID, c.name, c.description, c.membersNumber
            FROM CommunitiesUsers cu
            INNER JOIN Communities c ON c.communityID = cu.communityID
            WHERE cu.userID = @userId
            ORDER BY c.name;
            """;

        List<CommunityDto> communities = [];

        await using SqlConnection connection = new(connectionStrings.CommunityConnection);
        await using SqlCommand command = new(query, connection);
        command.Parameters.Add("@userId", SqlDbType.Int).Value = userId;

        await connection.OpenAsync();
        await using SqlDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            communities.Add(new CommunityDto(
                reader.GetInt32(reader.GetOrdinal("communityID")),
                reader.GetString(reader.GetOrdinal("name")),
                reader.GetString(reader.GetOrdinal("description")),
                reader.GetInt32(reader.GetOrdinal("membersNumber"))));
        }

        return communities;
    }
}

internal sealed class TinderRepository(ConnectionStrings connectionStrings)
{
    public async Task<DatingProfileDto?> GetProfileAsync(int userId)
    {
        const string query = """
            SELECT userId, name, location, age, bio, dateOfBirth
            FROM Profiles
            WHERE userId = @userId;
            """;

        await using SqlConnection connection = new(connectionStrings.DefaultConnection);
        await using SqlCommand command = new(query, connection);
        command.Parameters.Add("@userId", SqlDbType.Int).Value = userId;

        await connection.OpenAsync();
        await using SqlDataReader reader = await command.ExecuteReaderAsync();

        return await reader.ReadAsync() ? MapProfile(reader) : null;
    }

    public async Task<IReadOnlyList<DatingProfileDto>> GetDiscoverCandidatesAsync(int userId)
    {
        const string query = """
            SELECT TOP (25) userId, name, location, age, bio, dateOfBirth
            FROM Profiles
            WHERE userId <> @userId AND isArchived = 0
            ORDER BY boost DESC, name ASC;
            """;

        List<DatingProfileDto> profiles = [];

        await using SqlConnection connection = new(connectionStrings.DefaultConnection);
        await using SqlCommand command = new(query, connection);
        command.Parameters.Add("@userId", SqlDbType.Int).Value = userId;

        await connection.OpenAsync();
        await using SqlDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            profiles.Add(MapProfile(reader));
        }

        return profiles;
    }

    private static DatingProfileDto MapProfile(SqlDataReader reader)
    {
        return new DatingProfileDto(
            reader.GetInt32(reader.GetOrdinal("userId")),
            reader.GetString(reader.GetOrdinal("name")),
            reader.GetString(reader.GetOrdinal("location")),
            reader.GetInt32(reader.GetOrdinal("age")),
            reader.IsDBNull(reader.GetOrdinal("bio")) ? string.Empty : reader.GetString(reader.GetOrdinal("bio")),
            reader.GetDateTime(reader.GetOrdinal("dateOfBirth")));
    }
}

internal sealed record LoginRequest(string Email, string Password);
internal sealed record LoginResponse(int UserId, string Username, string Email);
internal sealed record UserDto(int UserId, string Username, string Email);
internal sealed record CommunityDto(int CommunityId, string Name, string Description, int MembersNumber);
internal sealed record DatingProfileDto(int UserId, string Name, string Location, int Age, string Bio, DateTime DateOfBirth);

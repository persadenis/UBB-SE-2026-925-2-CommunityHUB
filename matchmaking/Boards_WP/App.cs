using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Boards_WP.ViewModels;

namespace Boards_WP;

public sealed partial class App
{
    private static readonly Lazy<App> Instance = new(() => new App());
    private readonly object syncRoot = new();
    private IServiceProvider? services;
    private bool isInitialized;

    public static App Current => Instance.Value;
    public static IServiceProvider Services => Current.EnsureServices();

    public Page? M_window { get; private set; }
    public CommunityHostPage? HostPage { get; private set; }
    public Frame? CommunityContentFrame => HostPage?.ContentFrame;
    public event EventHandler? TinderNavigationRequested;

    public static T GetService<T>()
        where T : class
        => Services.GetRequiredService<T>();

    public void Initialize()
    {
        lock (syncRoot)
        {
            if (isInitialized)
            {
                return;
            }

            services = ConfigureServices();
            isInitialized = true;
            ResetSession(services);
        }
    }

    public void AttachHost(CommunityHostPage hostPage)
    {
        M_window = hostPage;
        HostPage = hostPage;
    }

    public void RequestTinderNavigation()
    {
        TinderNavigationRequested?.Invoke(this, EventArgs.Empty);
    }

    private IServiceProvider EnsureServices()
    {
        Initialize();
        return services!;
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        string connectionString = ResolveCommunityConnectionString();

        services.AddSingleton(connectionString);

        services.AddSingleton<MainViewModel>();

        services.AddSingleton<IBetsRepository, BetsRepository>();
        services.AddSingleton<ICommentsRepository, CommentsRepository>();
        services.AddSingleton<ICommunitiesRepository, CommunitiesRepository>();
        services.AddSingleton<INotificationRepository, NotificationRepository>();
        services.AddSingleton<IPostsRepository, PostsRepository>();
        services.AddSingleton<ITagsRepository, TagsRepository>();
        services.AddSingleton<IUsersMoodRepository, UsersMoodRepository>();
        services.AddSingleton<IUsersRepository, UsersRepository>();

        services.AddSingleton<IBetsService, BetsService>();
        services.AddSingleton<ICommentsService, CommentsService>();
        services.AddSingleton<ICommunitiesService, CommunitiesService>();
        services.AddSingleton<INotificationsService, NotificationsService>();
        services.AddSingleton<IPostsService, PostsService>();
        services.AddSingleton<IUsersService, UsersService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<UserSession>();

        services.AddSingleton<FeedViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<NotificationItemViewModel>();
        services.AddTransient<NotificationsListViewModel>();
        services.AddTransient<CreatePostViewModel>();
        services.AddSingleton<CommunityBarViewModel>();
        services.AddTransient<CreateCommunityViewModel>();
        services.AddTransient<UpdateCommunityViewModel>();
        services.AddSingleton<CommunityViewModel>();
        services.AddTransient<CreateTagViewModel>();
        services.AddTransient<CommentViewModel>();
        services.AddTransient<FullPostViewModel>();
        services.AddTransient<PostPreviewViewModel>();
        services.AddTransient<HeaderViewModel>();
        services.AddTransient<BetsViewModel>();
        services.AddTransient<BetItemViewModel>();
        services.AddTransient<CreateBetViewModel>();
        services.AddTransient<PlaceBetViewModel>();

        return services.BuildServiceProvider();
    }

    private static string ResolveCommunityConnectionString()
    {
        const string fallback =
            "Server=(localdb)\\MSSQLLocalDB;Database=Communities;Trusted_Connection=True;TrustServerCertificate=True;";

        string settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.Development.json");
        if (!File.Exists(settingsPath))
        {
            return fallback;
        }

        try
        {
            using JsonDocument json = JsonDocument.Parse(File.ReadAllText(settingsPath));
            if (json.RootElement.TryGetProperty("ConnectionStrings", out JsonElement connectionStrings)
                && connectionStrings.TryGetProperty("CommunityConnection", out JsonElement communityConnection)
                && communityConnection.ValueKind == JsonValueKind.String)
            {
                return communityConnection.GetString() ?? fallback;
            }
        }
        catch
        {
        }

        return fallback;
    }

    private static void ResetSession(IServiceProvider serviceProvider)
    {
        var session = serviceProvider.GetRequiredService<UserSession>();
        session.CurrentUser = new User
        {
            UserID = 0,
            Username = "Guest",
            Email = string.Empty,
            PasswordHash = string.Empty,
            Bio = string.Empty,
            Status = "Offline",
            AvatarUrl = "ms-appx:///Assets/DefaultAvatar.png"
        };
    }
}

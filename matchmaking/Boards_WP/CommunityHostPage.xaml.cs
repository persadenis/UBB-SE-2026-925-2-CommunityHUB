using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Boards_WP.ViewModels;

namespace Boards_WP;

public sealed partial class CommunityHostPage : Page
{
    private readonly MainViewModel mainViewModel;

    public MainViewModel MainViewModel => mainViewModel;

    public CommunityHostPage()
    {
        App.Current.Initialize();
        mainViewModel = App.GetService<MainViewModel>();

        InitializeComponent();

        App.Current.AttachHost(this);

        if (App.GetService<UserSession>().CurrentUser.UserID > 0)
        {
            mainViewModel.IsLoggedIn = true;
            LoginFrame.Visibility = Visibility.Collapsed;
            InitializeContentFrame();
        }
        else
        {
            mainViewModel.IsLoggedIn = false;
            var navigationService = App.GetService<INavigationService>();
            navigationService.Initialize(LoginFrame);
            navigationService.NavigateTo(typeof(Views.Pages.LoginView));
        }
    }

    public Visibility BoolToVis(bool isLoggedIn) => isLoggedIn ? Visibility.Visible : Visibility.Collapsed;

    public void NavigateToPage(Type pageType, object? parameter = null)
    {
        ContentFrame.Navigate(pageType, parameter);
    }

    public void InitializeContentFrame()
    {
        var navigationService = App.GetService<INavigationService>();
        navigationService.Initialize(ContentFrame);
        navigationService.NavigateTo(typeof(Views.Pages.FeedView));
    }
}

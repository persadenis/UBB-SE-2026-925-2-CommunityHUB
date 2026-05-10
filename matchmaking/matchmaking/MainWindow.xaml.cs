using Boards_WP.Data.Models;
using matchmaking.Domain;
using matchmaking.Repositories;
using matchmaking.Services;
using matchmaking.ViewModels;
using matchmaking.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.System;

namespace matchmaking
{
    public sealed partial class MainWindow : Window
    {
        private const string SecretCode = "DOUBT112";
        private string _typedBuffer = string.Empty;

        public MainWindow()
        {
            InitializeComponent();

            Boards_WP.App.Current.TinderNavigationRequested += HandleTinderNavigationRequested;
            RootFrame.Navigate(typeof(Boards_WP.CommunityHostPage));

            if (Content is UIElement rootElement)
            {
                rootElement.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(OnGlobalKeyDown), true);
            }
            RootFrame.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(OnGlobalKeyDown), true);
        }

        private void HandleTinderNavigationRequested(object? sender, System.EventArgs e)
        {
            UserSession userSession = Boards_WP.App.GetService<UserSession>();
            int userId = userSession.CurrentUser?.UserID ?? 0;
            if (userId <= 0)
            {
                return;
            }

            Frame targetFrame = Boards_WP.App.Current.CommunityContentFrame ?? RootFrame;
            OpenTinderForUser(targetFrame, userId, userSession.CurrentUser.Username);
        }

        private static void OpenTinderForUser(Frame targetFrame, int userId, string username)
        {
            ProfileRepository profileRepository = new ProfileRepository(App.ConnectionString);
            DatingAdminRepository datingAdminRepository = new DatingAdminRepository(App.ConnectionString);
            ProfileService profileService = new ProfileService(profileRepository);
            DatingAdminService datingAdminService = new DatingAdminService(datingAdminRepository);

            if (datingAdminService.IsAdmin(userId))
            {
                AdminViewModel adminViewModel = new AdminViewModel(
                    new SupportTicketService(new SupportTicketRepository(App.ConnectionString)),
                    profileService);
                targetFrame.Navigate(typeof(AdminView), adminViewModel);
                return;
            }

            DatingProfile? profile = profileService.GetProfileById(userId);
            if (profile == null)
            {
                CreateProfileViewModel createProfileViewModel = new CreateProfileViewModel(userId, profileService)
                {
                    Name = username
                };
                targetFrame.Navigate(typeof(CreateProfileView), createProfileViewModel);
                return;
            }

            SplashViewModel splashViewModel = new SplashViewModel(userId, profileService, datingAdminService);
            if (!splashViewModel.IsUserAdult())
            {
                targetFrame.Navigate(typeof(AgeBlockView));
                return;
            }

            MainViewModel mainViewModel = new MainViewModel(userId, App.ConnectionString, false);
            targetFrame.Navigate(typeof(MainView), mainViewModel);
        }

        private void OnGlobalKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.OriginalSource is DependencyObject source && IsTextInputSource(source))
            {
                return;
            }

            if (!TryMapKeyToSecretChar(e.Key, out char typedChar))
            {
                return;
            }

            _typedBuffer += typedChar;
            if (_typedBuffer.Length > SecretCode.Length)
            {
                _typedBuffer = _typedBuffer[^SecretCode.Length..];
            }

            if (_typedBuffer.EndsWith(SecretCode, System.StringComparison.OrdinalIgnoreCase))
            {
                _typedBuffer = string.Empty;
                OpenSpouseCheckerView();
            }
        }

        private static bool TryMapKeyToSecretChar(VirtualKey key, out char result)
        {
            if (key >= VirtualKey.A && key <= VirtualKey.Z)
            {
                result = (char)('A' + (key - VirtualKey.A));
                return true;
            }

            if (key >= VirtualKey.Number0 && key <= VirtualKey.Number9)
            {
                result = (char)('0' + (key - VirtualKey.Number0));
                return true;
            }

            if (key >= VirtualKey.NumberPad0 && key <= VirtualKey.NumberPad9)
            {
                result = (char)('0' + (key - VirtualKey.NumberPad0));
                return true;
            }

            result = default;
            return false;
        }

        private static bool IsTextInputSource(DependencyObject source)
        {
            DependencyObject? current = source;
            while (current != null)
            {
                if (current is TextBox || current is PasswordBox || current is RichEditBox)
                {
                    return true;
                }

                current = VisualTreeHelper.GetParent(current);
            }

            return false;
        }

        private void OpenSpouseCheckerView()
        {
            SupportTicketRepository supportTicketRepository = new SupportTicketRepository(App.ConnectionString);
            SupportTicketService supportTicketService = new SupportTicketService(supportTicketRepository);
            SpouseCheckerViewModel spouseCheckerViewModel = new SpouseCheckerViewModel(supportTicketService);

            RootFrame.Navigate(typeof(SpouseCheckerView), spouseCheckerViewModel);
        }
    }
}

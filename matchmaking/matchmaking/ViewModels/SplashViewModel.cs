using matchmaking.Domain;
using matchmaking.Services;
using matchmaking.Utils;
using System;
using System.Threading.Tasks;

namespace matchmaking.ViewModels
{
    internal class SplashViewModel : ObservableObject
    {
        private readonly int _userId;
        private readonly ProfileService _profileService;
        private readonly DatingAdminService _datingAdminService;

        public RelayCommand NavigateCommand { get; }

        private Screen _nextScreen;
        public Screen NextScreen
        {
            get => _nextScreen;
            private set => SetProperty(ref _nextScreen, value);
        }

        public SplashViewModel(int userId, ProfileService profileService, DatingAdminService datingAdminService)
        {
            _userId = userId;
            _profileService = profileService;
            _datingAdminService = datingAdminService;

            NavigateCommand = new RelayCommand(ExecuteNavigation, CanNavigate);
        }

        private bool CanNavigate() => true;

        private void ExecuteNavigation()
        {
            NextScreen = DecideNextScreen();
        }

        public int UserId => _userId;

        public bool IsUserAdult()
        {
            DatingProfile profile = _profileService.GetProfileById(_userId);
            if (profile == null)
            {
                return true;
            }

            DateTime today = DateTime.Today;
            int age = today.Year - profile.DateOfBirth.Year;

            if (profile.DateOfBirth.Date > today.AddYears(-age))
                age--;

            return age >= 18;
        }

        public bool HasProfile()
        {
            try
            {
                DatingProfile profile = _profileService.GetProfileById(_userId);
                return profile != null;
            }
            catch
            {
                return false;
            }
        }

        public bool IsAdmin()
        {
            return _datingAdminService.IsAdmin(_userId);
        }

        public Screen DecideNextScreen()
        {
            if (IsAdmin()) return Screen.ADMIN;
            if (!HasProfile()) return Screen.CREATE;
            if (!IsUserAdult()) return Screen.AGE_BLOCK;

            return Screen.DISCOVER;
        }
    }
}

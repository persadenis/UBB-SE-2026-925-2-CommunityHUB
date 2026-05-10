using Boards_WP.Data.Models;

namespace Boards_WP.Data.Services.Interfaces
{
    public interface IUsersService
    {
        User GetUserByID(int userID);
        User Login(string email, string password);
    }
}
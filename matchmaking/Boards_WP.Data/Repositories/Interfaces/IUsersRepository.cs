using Boards_WP.Data.Models;

namespace Boards_WP.Data.Repositories.Interfaces
{
    public interface IUsersRepository
    {
        User GetUserByID(int userID);
        User GetUserByEmail(string email);
    }
}
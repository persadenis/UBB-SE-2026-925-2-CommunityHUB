using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Boards_WP.Data.Models;

namespace Boards_WP.Data.Repositories
{
    public interface IBetsRepository
    {
        public int AddBet(Bet b);
        public void RemoveBet(int BetID);
        public Bet GetBetByID(int BetID);
        public List<Bet> GetAllBetsSortedByDate();
        public List<Bet> GetBetsByKeywords(String Keywords);
        public void UpdateBetAmounts(int BetID, int YesAmount, int NoAmount);
        public UsersBets GetUserBetByID(int UserID, int BetID);
        public List<UsersBets> GetUserBetsByUser(int UserID);
        public List<UsersBets> GetUserBetsByBet(int BetID);
        public void AddUserBet(UsersBets UserBet);
        public UsersTokens GetUserTokens(int UserID);
        public Boolean UserTokensExist(int UserID);
        public void AddUserTokens(UsersTokens userToken);
        public void UpdateUserTokens(int UserID, int NewAmount);
    }
}

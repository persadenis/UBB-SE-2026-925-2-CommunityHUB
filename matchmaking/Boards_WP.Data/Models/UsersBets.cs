using System;

namespace Boards_WP.Data.Models
{
    public class UsersBets
    {
        public required User BettingUser { get; set; }
        public required Bet SelectedBet { get; set; }
        public int Amount { get; set; }
        public decimal Odd { get; set; }
        public BetVote Vote { get; set; }

    }
}

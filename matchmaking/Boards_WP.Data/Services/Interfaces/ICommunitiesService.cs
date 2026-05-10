using System;
using System.Collections.Generic;
using Boards_WP.Data.Models;

namespace Boards_WP.Data.Services
{

    public interface ICommunitiesService
    {
        public List<Community> searchCommunities(String Match);
        public void AddUser(int CommunityID, int UserID);
        public void RemoveUser(int CommunityID, int UserID);
        public void AddCommunity(Community AddedCommunity);
        public void UpdateCommunityInfo(int CommunityID, String Description, byte[] NewCommunityPicture, byte[] NewBanner);
        public Boolean CheckOwner(int CommunityID, int UserID);
        public List<Community> GetCommunitiesUserIsPartOf(int UserID);
        public Boolean IsPartOfCommunity(int UserID, int CommunityID);
        public ThemeColor DetermineCommunityThemeColor(int CommunityID);

        public Community GetCommunityByID(int CommunityID);
    }
}

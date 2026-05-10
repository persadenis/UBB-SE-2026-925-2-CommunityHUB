using System;
using System.Collections.Generic;

using Boards_WP.Data.Models;

public interface ICommunitiesRepository
{
    public int AddCommunity(Community NewCommunity);
    public void UpdateDescription(int CommunityID, String NewDescription);
    public void UpdateCommunityPicture(int CommunityID, byte[] NewPhoto);
    public void UpdateBanner(int CommunityID, byte[] NewBanner);
    public void IncreaseMembersNumber(int CommunityID);
    public void DecreaseMembersNumber(int CommunityID);
    public Boolean CheckOwner(int CommunityID, int OwnerID);
    public List<Community> GetCommunitiesByNamesMatch(String Name);
    public List<Community> GetCommunitiesUserIsPartOf(int UserID);
    public Boolean IsPartOfCommunity(int UserID, int CommunityID);
    public void AddUserToCommunity(int CommunityID, int UserID);
    public void RemoveUserFromCommunity(int CommunityID, int UserID);

    public Community? GetCommunityByID(int CommunityID);

}

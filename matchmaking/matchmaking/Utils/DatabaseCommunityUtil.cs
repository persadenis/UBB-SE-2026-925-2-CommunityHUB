using Boards_WP.Data.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace matchmaking.Utils
{
    internal class DatabaseCommunityUtil : ICommunityLookup
    {
        public List<string> GetSharedCommunities(int userId1, int userId2)
        {
            try
            {
                var communitiesService = Boards_WP.App.GetService<ICommunitiesService>();
                var firstUserCommunities = communitiesService
                    .GetCommunitiesUserIsPartOf(userId1)
                    .Select(community => community.Name)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                return communitiesService
                    .GetCommunitiesUserIsPartOf(userId2)
                    .Select(community => community.Name)
                    .Where(firstUserCommunities.Contains)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}

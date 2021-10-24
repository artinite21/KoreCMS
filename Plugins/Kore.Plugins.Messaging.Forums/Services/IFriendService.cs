using Kore.Collections.Generic;
using Kore.Plugins.Messaging.Forums.Data.Domain;
using System.Threading.Tasks;

namespace Kore.Plugins.Messaging.Forums.Services
{
    public interface IFriendService
    {
        Task DeleteFriendAsync(Friend friend);

        Task<Friend> GetFriendByIdAsync(string userId, string friendId);

        Task<IPagedList<Friend>> GetAllFriendsAsync(
            string userId,
            int pageIndex = 0,
            int pageSize = int.MaxValue);

        Task InsertFriendAsync(Friend friend);
    }
}
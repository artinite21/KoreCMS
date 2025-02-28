﻿using Kore.Collections.Generic;
using Kore.Plugins.Messaging.Forums.Data.Domain;
using System.Threading.Tasks;

namespace Kore.Plugins.Messaging.Forums.Services
{
    public interface IUserManagementService
    {
        Task DeleteFriendAsync(Friend friend);

        Task DeleteFriendsByUserIdAsync(string userId);

        Task<Friend> GetFriendByIdAsync(string userId, string friendId);

        Task<IPagedList<Friend>> GetAllFriendsAsync(
            string userId,
            int pageIndex = 0,
            int pageSize = int.MaxValue);

        Task InsertFriendAsync(Friend friend);

        Task<BlockedUser> GetBlockedUserByIdAsync(string blockedByUserId, string blockedUserId, bool? isBlocked = default);

        Task<IPagedList<BlockedUser>> GetAllBlockedUsersAsync(
            string blockedByUserId,
            int pageIndex = 0,
            int pageSize = int.MaxValue);

        Task InsertBlockedUserAsync(BlockedUser blockedUser);

        Task UpdateBlockedUserAsync(BlockedUser blockedUser);

        Task DeleteBlockedUsersByUserIdAsync(string userId);
    }
}
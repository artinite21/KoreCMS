using Kore.Collections.Generic;
using Kore.Data;
using Kore.Exceptions;
using Kore.Plugins.Messaging.Forums.Data.Domain;
using Kore.Security.Membership;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kore.Plugins.Messaging.Forums.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IRepository<BlockedUser> blockedUserRepository;
        private readonly IRepository<Friend> friendRepository;
        private readonly IMembershipService membershipService;

        public UserManagementService(IRepository<BlockedUser> blockedUserRepository,
            IRepository<Friend> friendRepository, 
            IMembershipService membershipService)
        {
            this.blockedUserRepository = blockedUserRepository;
            this.friendRepository = friendRepository;
            this.membershipService = membershipService;
        }

        public async Task DeleteFriendAsync(Friend friend)
        {
            if (friend == null)
            {
                throw new ArgumentNullException("friend");
            }

            await friendRepository.DeleteAsync(friend);
        }

        public async Task DeleteFriendsByUserIdAsync(string userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException("friend");
            }

            await friendRepository.DeleteAsync(x => x.UserId == userId || x.FriendId == userId);
        }

        public async Task<IPagedList<Friend>> GetAllFriendsAsync(string userId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            using (var friendConnection = friendRepository.OpenConnection())
            {
                var query = friendConnection.Query();

                if (!string.IsNullOrEmpty(userId))
                {
                    query = query.Where(f => f.UserId == userId);
                }
                query = query.OrderByDescending(f => f.UserId);

                return await Task.FromResult(new PagedList<Friend>(query, pageIndex, pageSize));
            }
        }

        public async Task<Friend> GetFriendByIdAsync(string userId, string friendId)
        {
            Friend friend;
            using (var connection = friendRepository.OpenConnection())
            {
                friend = await connection.Query(x => x.UserId == userId && x.FriendId == friendId).FirstOrDefaultAsync();
            }

            if (friend == null)
            {
                return null;
            }
            return friend;
        }

        public async Task InsertFriendAsync(Friend friend)
        {
            if (friend == null)
            {
                throw new ArgumentNullException("friend");
            }

            await friendRepository.InsertAsync(friend);

            // May need to check if user still exists
            //var userTo = await membershipService.GetUserById(privateMessage.ToUserId);
            //if (userTo == null)
            //{
            //    throw new KoreException("Recipient could not be loaded");
            //}
        }

        public async Task InsertBlockedUserAsync(BlockedUser blockedUser)
        {
            if (blockedUser == null)
            {
                throw new ArgumentNullException("blockedUser");
            }

            await blockedUserRepository.InsertAsync(blockedUser);

            var blockedUserTo = await membershipService.GetUserById(blockedUser.BlockedUserId);
            if (blockedUserTo == null)
            {
                throw new KoreException("Recipient could not be loaded");
            }
        }

        public async Task UpdateBlockedUserAsync(BlockedUser blockedUser)
        {
            if (blockedUser == null)
            {
                throw new ArgumentNullException("blockedUser");
            }

            await blockedUserRepository.UpdateAsync(blockedUser);
            //event notification
            //_eventPublisher.EntityDeleted(blockedUser);  
        }

        public async Task DeleteBlockedUsersByUserIdAsync(string userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException("blockedUser");
            }

            await blockedUserRepository.DeleteAsync(x => x.BlockedByUserId == userId || x.BlockedUserId == userId);
        }

        public async Task<BlockedUser> GetBlockedUserByIdAsync(string blockedByUserId, string blockedUserId, bool? isBlocked = default)
        {
            BlockedUser blockedUser;
            using (var connection = blockedUserRepository.OpenConnection())
            {
                if (isBlocked.HasValue)
                {
                    blockedUser = await connection.Query(x => x.BlockedByUserId == blockedByUserId && x.BlockedUserId == blockedUserId && x.IsBlocked == isBlocked).FirstOrDefaultAsync();
                }
                else
                {
                    blockedUser = await connection.Query(x => x.BlockedByUserId == blockedByUserId && x.BlockedUserId == blockedUserId).FirstOrDefaultAsync();
                }
            }

            if (blockedUser == null)
            {
                return null;
            }

            return blockedUser;
        }

        public async Task<IPagedList<BlockedUser>> GetAllBlockedUsersAsync(string blockedByUserId, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            // List<BlockedUser> blockedUsers = new List<BlockedUser>();         

            using (var blockedUserConnection = blockedUserRepository.OpenConnection())
            {
                var query = blockedUserConnection.Query();

                if (!string.IsNullOrEmpty(blockedByUserId))
                {
                    //blockedUsers = await query.Where(bu => bu.BlockedByUserId == blockedByUserId).ToListAsync();
                    query = query.Where(bu => bu.BlockedByUserId == blockedByUserId);
                }
                query = query.OrderByDescending(bu => bu.BlockedByUserId);

                return await Task.FromResult(new PagedList<BlockedUser>(query, pageIndex, pageSize));
            }
        }
    }
}
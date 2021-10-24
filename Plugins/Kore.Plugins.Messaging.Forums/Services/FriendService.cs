using Kore.Collections.Generic;
using Kore.Data;
using Kore.Plugins.Messaging.Forums.Data.Domain;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kore.Plugins.Messaging.Forums.Services
{
    public class FriendService : IFriendService
    {
        private readonly IRepository<Friend> friendRepository;

        public FriendService(IRepository<Friend> friendRepository)
        {
            this.friendRepository = friendRepository;
        }

        public async Task DeleteFriendAsync(Friend friend)
        {
            if (friend == null)
            {
                throw new ArgumentNullException("friend");
            }

            await friendRepository.DeleteAsync(friend);
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
    }
}
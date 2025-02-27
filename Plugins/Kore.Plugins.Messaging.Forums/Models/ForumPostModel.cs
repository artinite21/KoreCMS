﻿using System;

namespace Kore.Plugins.Messaging.Forums.Models
{
    public class ForumPostModel
    {
        public int Id { get; set; }

        public int ForumTopicId { get; set; }

        public string ForumTopicSeName { get; set; }

        public string FormattedText { get; set; }

        public bool IsCurrentUserAllowedToEditPost { get; set; }

        public bool IsCurrentUserAllowedToDeletePost { get; set; }

        public string UserId { get; set; }

        public int? ParentPostId { get; set; }

        public string ParentUserName { get; set; }

        public int PostNumber { get; set; }

        public bool AllowViewingProfiles { get; set; }

        public string UserAvatarUrl { get; set; }

        public bool IsAvatarApproved { get; set; }

        public bool HasBlockedUser { get; set; }

        public bool BlockedByUser { get; set; }

        public bool FlaggedByUser { get; set; }

        public bool IsFlagged { get; set; }

        public bool IsFriend { get; set; }

        public int FlagCount { get; set; }

        public bool IsEdited { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsRegisteredUser { get; set; }

        public bool IsTopicLocked { get; set; }

        public string UserName { get; set; }

        public string UserRoleName { get; set; }

        public bool IsUserForumModerator { get; set; }

        public bool IsCurrentUserForumModerator { get; set; }

        public string PostEditedOnStr { get; set; }

        public string PostCreatedOnStr { get; set; }

        public bool ShowUsersPostCount { get; set; }

        public int ForumPostCount { get; set; }

        public bool ShowUsersJoinDate { get; set; }

        public DateTime UserJoinDate { get; set; }

        public bool ShowUsersLocation { get; set; }

        public string UserLocation { get; set; }

        public bool AllowPrivateMessages { get; set; }

        public bool SignaturesEnabled { get; set; }

        public string FormattedSignature { get; set; }

        public int CurrentTopicPage { get; set; }
    }
}
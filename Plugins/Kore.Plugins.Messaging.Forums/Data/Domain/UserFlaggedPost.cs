using System.Data.Entity.ModelConfiguration;
using Kore.Data;
using Kore.Data.EntityFramework;
using Kore.Web.Plugins;

namespace Kore.Plugins.Messaging.Forums.Data.Domain
{
    public class UserFlaggedPost : IEntity
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int TopicId { get; set; }

        public int PostId { get; set; }

        public bool FlaggedByUser { get; set; }

        public virtual ForumPost ForumPost { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class UserFlaggedPostMap : EntityTypeConfiguration<UserFlaggedPost>, IEntityTypeConfiguration
    {
        public UserFlaggedPostMap()
        {
            this.ToTable(Constants.Tables.UserFlaggedPosts);
            this.HasKey(x => x.Id);
            this.Property(x => x.UserId).IsRequired().HasMaxLength(128).IsUnicode(true);
            this.Property(x => x.TopicId).IsRequired();
            this.Property(x => x.PostId).IsRequired();
            this.Property(x => x.FlaggedByUser).IsRequired();

            this.HasRequired(x => x.ForumPost).WithMany().HasForeignKey(x => x.PostId);
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return PluginManager.IsPluginInstalled(Constants.PluginSystemName); }
        }

        #endregion IEntityTypeConfiguration Members
    }
}
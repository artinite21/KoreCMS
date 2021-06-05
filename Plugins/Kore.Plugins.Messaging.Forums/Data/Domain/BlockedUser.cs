using Kore.Data;
using Kore.Data.EntityFramework;
using Kore.Web.Plugins;
using System.Data.Entity.ModelConfiguration;

namespace Kore.Plugins.Messaging.Forums.Data.Domain
{
    public class BlockedUser : IEntity
    {
        public int Id { get; set; }

        public string BlockedByUserId { get; set; }

        public string BlockedUserId { get; set; }

        public bool IsBlocked { get; set; }


        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class BlockedUserMap : EntityTypeConfiguration<BlockedUser>, IEntityTypeConfiguration
    {
        public BlockedUserMap()
        {
            this.ToTable(Constants.Tables.BlockedUsers);
            this.HasKey(x => x.Id);
            this.Property(x => x.BlockedByUserId).IsRequired().HasMaxLength(128).IsUnicode(true);
            this.Property(x => x.BlockedUserId).IsRequired().HasMaxLength(128).IsUnicode(true);
            this.Property(x => x.IsBlocked).IsRequired();
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return PluginManager.IsPluginInstalled(Constants.PluginSystemName); }
        }

        #endregion IEntityTypeConfiguration Members
    }
}

using Kore.Data;
using Kore.Data.EntityFramework;
using System.Data.Entity.ModelConfiguration;

namespace Kore.Plugins.Messaging.Forums.Data.Domain
{
    public class Friend : IEntity
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string FriendId { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class FriendMap : EntityTypeConfiguration<Friend>, IEntityTypeConfiguration
    {
        public FriendMap()
        {
            this.ToTable(Constants.Tables.Friends);
            this.HasKey(x => x.Id);
            this.Property(x => x.UserId).IsRequired().HasMaxLength(128).IsUnicode(true);
            this.Property(x => x.FriendId).IsRequired().HasMaxLength(128).IsUnicode(true);
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return true; }
        }

        #endregion IEntityTypeConfiguration Members
    }
}
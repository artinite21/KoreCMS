using System.Data.Entity.ModelConfiguration;
using Kore.Data;
using Kore.Data.EntityFramework;
using Kore.Web.Plugins;

namespace Kore.Plugins.Messaging.Forums.Data.Domain
{
    public class ReportedUser : IEntity
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public string ReportedUserId { get; set; }

        public string Subject { get; set; }

        public string Details { get; set; }

        #region IEntity Members

        public object[] KeyValues
        {
            get { return new object[] { Id }; }
        }

        #endregion IEntity Members
    }

    public class ReportedUserMap : EntityTypeConfiguration<ReportedUser>, IEntityTypeConfiguration
    {
        public ReportedUserMap()
        {
            this.ToTable(Constants.Tables.ReportedUsers);
            this.HasKey(x => x.Id);
            this.Property(x => x.UserId).IsRequired().HasMaxLength(128).IsUnicode(true);
            this.Property(x => x.ReportedUserId).IsRequired().HasMaxLength(128).IsUnicode(true);
            this.Property(x => x.Subject).IsRequired().HasMaxLength(512).IsUnicode(true);
            this.Property(x => x.Details).IsRequired().IsMaxLength().IsUnicode(true);
        }

        #region IEntityTypeConfiguration Members

        public bool IsEnabled
        {
            get { return PluginManager.IsPluginInstalled(Constants.PluginSystemName); }
        }

        #endregion IEntityTypeConfiguration Members
    }
}
﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kore.Infrastructure;
using Kore.Security.Membership;

namespace Kore.Web.Security.Membership
{
    public class MobileUserProfileProvider : IUserProfileProvider
    {
        public const string PropertyDontUseMobileVersion = "DontUseMobileVersion";

        [Display(Name = "Don't Use Mobile Version")]
        public bool DontUseMobileVersion { get; set; }

        #region IUserProfileProvider Members

        public string Name
        {
            get { return "Mobile"; }
        }

        public string DisplayTemplatePath
        {
            get { return "Kore.Web.Views.Shared.DisplayTemplates.MobileUserProfileProvider"; }
        }

        public string EditorTemplatePath
        {
            get { return "Kore.Web.Views.Shared.EditorTemplates.MobileUserProfileProvider"; }
        }

        public IEnumerable<string> GetFieldNames()
        {
            return new[]
            {
                PropertyDontUseMobileVersion
            };
        }

        public void PopulateFields(string userId)
        {
            var membershipService = EngineContext.Current.Resolve<IMembershipService>();
            string dontUseMobileVersion = membershipService.GetProfileEntry(userId, PropertyDontUseMobileVersion);
            DontUseMobileVersion = !string.IsNullOrEmpty(dontUseMobileVersion) && bool.Parse(dontUseMobileVersion);
        }

        #endregion IUserProfileProvider Members
    }
}
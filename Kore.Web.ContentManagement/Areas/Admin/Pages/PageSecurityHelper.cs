﻿using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Kore.Infrastructure;
using Kore.Security.Membership;
using Kore.Web.ContentManagement.Areas.Admin.Blog;
using Kore.Web.ContentManagement.Areas.Admin.Pages.Domain;
using Newtonsoft.Json.Linq;

namespace Kore.Web.ContentManagement.Areas.Admin.Pages
{
    public static class PageSecurityHelper
    {
        public static async Task<bool> CheckUserHasAccessToBlog(IPrincipal user)
        {
            var blogSettings = EngineContext.Current.Resolve<BlogSettings>();

            if (!string.IsNullOrEmpty(blogSettings.AccessRestrictions))
            {
                dynamic accessRestrictions = JObject.Parse(blogSettings.AccessRestrictions);
                string roleIds = accessRestrictions.Roles;

                if (!string.IsNullOrEmpty(roleIds))
                {
                    var membershipService = EngineContext.Current.Resolve<IMembershipService>();
                    var roles = await membershipService.GetRolesByIds(roleIds.Split(','));

                    var roleNames = roles
                        .Select(x => x.Name)
                        .Where(x => x != null)
                        .ToList();

                    bool authorized = roleNames.Any(x => user.IsInRole(x));
                    if (!authorized)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static async Task<bool> CheckUserHasAccessToPage(Page page, IPrincipal user)
        {
            if (!page.IsEnabled)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(page.AccessRestrictions))
            {
                dynamic accessRestrictions = JObject.Parse(page.AccessRestrictions);
                string roleIds = accessRestrictions.Roles;

                if (!string.IsNullOrEmpty(roleIds))
                {
                    var membershipService = EngineContext.Current.Resolve<IMembershipService>();
                    var roles = await membershipService.GetRolesByIds(roleIds.Split(','));

                    var roleNames = roles
                        .Select(x => x.Name)
                        .Where(x => x != null)
                        .ToList();

                    bool authorized = roleNames.Any(x => user.IsInRole(x));
                    if (!authorized)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
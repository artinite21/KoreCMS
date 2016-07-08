﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Kore.Localization.Services;
using Kore.Web.Configuration;
using Kore.Web.ContentManagement.Areas.Admin.Localization.Models;
using Kore.Web.Mvc;
using Kore.Web.Mvc.Optimization;
using Kore.Web.Security.Membership.Permissions;
using Newtonsoft.Json.Linq;

namespace Kore.Web.ContentManagement.Areas.Admin.Localization.Controllers
{
    [Authorize]
    [RouteArea(CmsConstants.Areas.Localization)]
    [RoutePrefix("localizable-strings")]
    public class LocalizableStringController : KoreController
    {
        private readonly Lazy<ILanguageService> languageService;
        private readonly Lazy<ILocalizableStringService> localizableStringService;
        private readonly KoreSiteSettings siteSettings;

        public LocalizableStringController(
            Lazy<ILanguageService> languageService,
            Lazy<ILocalizableStringService> localizableStringService,
            KoreSiteSettings siteSettings)
        {
            this.languageService = languageService;
            this.localizableStringService = localizableStringService;
            this.siteSettings = siteSettings;
        }

        [Compress]
        [OutputCache(Duration = 86400, VaryByParam = "none")]
        [Route("")]
        public ActionResult Index()
        {
            if (! CheckPermission(StandardPermissions.FullAccess))
            {
                return new HttpUnauthorizedResult();
            }

            //var language = languageService.Value.FindOne(languageId);

            WorkContext.Breadcrumbs.Add(T(KoreCmsLocalizableStrings.Localization.Languages), Url.Action("Index", "Language"));
            //WorkContext.Breadcrumbs.Add(language.Name);
            WorkContext.Breadcrumbs.Add(T(KoreCmsLocalizableStrings.Localization.LocalizableStrings));

            ViewBag.Title = T(KoreCmsLocalizableStrings.Localization.Title);
            ViewBag.SubTitle = T(KoreCmsLocalizableStrings.Localization.LocalizableStrings);

            //ViewBag.CultureCode = language.CultureCode;

            return PartialView("Kore.Web.ContentManagement.Areas.Admin.Localization.Views.LocalizableString.Index");
        }

        [OutputCache(Duration = 86400, VaryByParam = "none")]
        [Route("get-translations")]
        public JsonResult GetTranslations()
        {
            string json = string.Format(
@"{{
    Columns: {{
        InvariantValue: '{0}',
        Key: '{1}',
        LocalizedValue: '{2}',
    }}
}}",
   T(KoreCmsLocalizableStrings.Localization.LocalizableStringModel.InvariantValue),
   T(KoreCmsLocalizableStrings.Localization.LocalizableStringModel.Key),
   T(KoreCmsLocalizableStrings.Localization.LocalizableStringModel.LocalizedValue));

            return Json(JObject.Parse(json), JsonRequestBehavior.AllowGet);
        }

        [Compress]
        [Route("export/{cultureCode}")]
        public ActionResult ExportLanguagePack(string cultureCode)
        {
            var localizedStrings = localizableStringService.Value.Find(x =>
                x.CultureCode == cultureCode &&
                x.TextValue != null);

            var languagePack = new LanguagePackFile
            {
                CultureCode = cultureCode,
                LocalizedStrings = localizedStrings.ToDictionary(k => k.TextKey, v => v.TextValue)
            };

            string json = languagePack.ToJson();
            string fileName = string.Format("{0}_LanguagePack_{1}_{2:yyyy-MM-dd}.json", siteSettings.SiteName, cultureCode, DateTime.Now);
            return File(new UTF8Encoding().GetBytes(json), "application/json", fileName);
        }

        //TODO: Test
        [Compress]
        [Route("translate/{key}")]
        public JsonResult Translate(string key)
        {
            return Json(new { Translation = T(key).Text });
        }
    }
}
using System.Web;

namespace Kore.Web.Services
{
    public interface IAuthenticationService
    {
        string GetToken(HttpContextBase httpContext);

        string GenerateJwtToken(string userId, string userName, HttpResponseBase httpResponseBase);

        bool ValidateToken(HttpContextBase httpContext);
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection
{
   public static class CookieExtensions
   {
      private const SameSiteMode Unspecified = (SameSiteMode) (-1);
      public static IServiceCollection ConfigureLaxCookies(this IServiceCollection services)
      {
         services.Configure<CookiePolicyOptions>(options =>
         {
            options.MinimumSameSitePolicy = Unspecified;
            options.OnAppendCookie = cookieContext => cookieContext.CookieOptions.SameSite = SameSiteMode.Lax;
            options.OnDeleteCookie = cookieContext => cookieContext.CookieOptions.SameSite = SameSiteMode.Lax;
         });

         return services;
      }

   }
}

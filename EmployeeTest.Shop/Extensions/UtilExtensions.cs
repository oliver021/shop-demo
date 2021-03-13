using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http
{
    public static class UtilExtensions
    {
        public static ulong GetUserId(this HttpContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var claim = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
            return ulong.Parse(claim.Value);
        }

        public static bool IsAdmin(this HttpContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        }
    }
}

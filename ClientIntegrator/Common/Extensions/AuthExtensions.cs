using ClientIntegrator.DataAccess.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace ClientIntegrator.Common.Extensions
{
    public static class AuthExtensions
    {
        public static bool IsAdmin(this ClaimsPrincipal loggedInUser)
        {
            if (loggedInUser.IsInRole("Admin"))
            {
                return true;
            }

            return false;
        }
        public static bool IsSuperUser(this ClaimsPrincipal loggedInUser)
        {
            if (loggedInUser.IsInRole("SuperUser"))
            {
                return true;
            }

            return false;
        }

        public static long GetOrganizationId(this ClaimsPrincipal loggedInUser)
        {
            if (loggedInUser.IsInRole("Admin"))
            {
                var ehrOrganizationIdClaim = loggedInUser.Claims.FirstOrDefault(x => x.Type == "OrganizationId");

                if (ehrOrganizationIdClaim != null)
                {
                    return long.Parse(ehrOrganizationIdClaim.Value);
                }
            }

            return 0;
        }
        public static bool HasPermission(this ClaimsPrincipal loggedInUser, PortalUser portalUser)
        {
            if (loggedInUser.IsInRole("SuperUser"))
            {
                return true;
            }

            if (loggedInUser.IsInRole("Admin"))
            {
                var ehrOrganizationIdClaim = loggedInUser.Claims.FirstOrDefault(x => x.Type == "OrganizationId");

                if (ehrOrganizationIdClaim != null)
                {
                    var ehrOrganizationId = long.Parse(ehrOrganizationIdClaim.Value);

                    return portalUser.OrganizationId == ehrOrganizationId;
                }
            }

            return false;
        }

        public static T GetLoggedInUserId<T>(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var loggedInUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(loggedInUserId, typeof(T));
            }
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(long))
            {
                return loggedInUserId != null ? (T)Convert.ChangeType(loggedInUserId, typeof(T)) : (T)Convert.ChangeType(0, typeof(T));
            }
            else
            {
                throw new Exception("Invalid type provided");
            }
        }

        public static string GetLoggedInUserName(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.FindFirstValue(ClaimTypes.Name);
        }

        public static long GetLoggedInUserOrganizationId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            var organizationIdClaim = principal.Claims.FirstOrDefault(x => x.Type == "OrganizationId");

            if (organizationIdClaim != null)
            {
                var organizationId = long.Parse(organizationIdClaim.Value);

                return organizationId;
            }

            return 0;
        }
    }
}

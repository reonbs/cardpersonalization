using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ZenithCardRepo.Services.BLL.Infrastructure
{
    public static class IdentityExtensions
    {
        public static string GetRegistrationType(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(Utilities.RegistrationType);

            if (claim == null)
                return "";

            return claim.Value;
        }
        public static string GetOrganisationCode(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(Utilities.OrganisationCode);

            if (claim == null)
                return "";

            return claim.Value;
        }

        public static string GetInstitutionID(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(Utilities.InstitutionID);

            if (claim == null)
                return "";

            return claim.Value;
        }

        public static string GetName(this IIdentity identity)
        {
            ClaimsIdentity claimsIdentity = identity as ClaimsIdentity;
            Claim claim = claimsIdentity?.FindFirst(ClaimTypes.Name);

            return claim?.Value ?? string.Empty;
        }
    }
}

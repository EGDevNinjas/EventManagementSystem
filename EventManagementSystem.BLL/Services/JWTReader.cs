using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.BLL.Services
{
    public class JWTReader
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            // read user identity & calims & id of the user
            var identity = user.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return -1; // if no user 
            }
            var claim = identity.Claims.FirstOrDefault(c => c.Type.ToLower() == "id");
            if (claim == null) return -1; // 

            int id;
            try
            {
                id = int.Parse(claim.Value);
            }
            catch (Exception)
            {
                return 0;
            }
            return id;
        } // not practical cause there is another controllers may have to read the Token


        public static string GetUserRole(ClaimsPrincipal user)
        {
            // read user identity & calims & id of the user
            var identity = user.Identity as ClaimsIdentity;
            if (identity == null)
            {
                return ""; // if no user 
            }
            var claim = identity.Claims.FirstOrDefault(c => c.Type.ToLower().Contains("Role")); // read the role from Token
            if (claim == null) return "";

            int id;
            try
            {
                id = int.Parse(claim.Value);
            }
            catch (Exception)
            {
                return "";
            }
            return claim.Value;
        }

        public static Dictionary<string, string> GetUserClaims(ClaimsPrincipal user)
        {
            Dictionary<string, string> claims = new Dictionary<string, string>();

            var identity = user.Identity as ClaimsIdentity;
            if (identity != null)
            {
                foreach (var claim in identity.Claims)
                {
                    claims.Add(claim.Type, claim.Value);
                }
            }
            return claims;
        }
    }
}

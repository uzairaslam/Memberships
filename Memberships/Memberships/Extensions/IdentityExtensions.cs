using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Memberships.Models;

namespace Memberships.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetUserFirstName(this IIdentity identity)
        {
            var db = ApplicationDbContext.Create();
            var user = db.Users.FirstOrDefault(u => u.UserName.Equals(identity.Name));
            return user != null ? user.FirstName : String.Empty;
        }
    }
}
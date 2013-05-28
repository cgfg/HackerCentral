using HackerCentral.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace HackerCentral.Extensions
{
    public static class UserRoleHelper
    {
        public static IEnumerable<T> ParseEnum<T>(string[] list) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            return list.Where(s =>
                    {
                        T role;
                        return Enum.TryParse(s.Trim(), true, out role);
                    })
                    .Select(s =>
                    {
                        T role;
                        Enum.TryParse(s.Trim(), true, out role);
                        return role;
                    })
                    .Distinct();
        }

        public static IEnumerable<UserRole> GetUserRoles(string username)
        {
            return ParseEnum<UserRole>(Roles.GetRolesForUser(username));
        }

        public static bool IsUserBlocked(string username)
        {
            return GetUserRoles(username).Any(s => s == UserRole.Blocked);
        }

        public static UserRole GetPrimaryUserRole(string username)
        {
            // there should only be one primary user role
            return (UserRole)ParseEnum<PrimaryUserRole>(Roles.GetRolesForUser(username)).Single();
        }

        public static IEnumerable<UserRole> GetSecondaryUserRoles(string username)
        {
            return ParseEnum<SecondaryUserRole>(Roles.GetRolesForUser(username)).Cast<UserRole>();
        }
    }
}
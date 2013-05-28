using DotNetOpenAuth.AspNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using HackerCentral.Models;

namespace HackerCentral.Extensions
{
    public static class AuthenticationExtensions
    {
        public static string GetFullName(this AuthenticationResult result)
        {

            AuthProvider provider;
            if (Enum.TryParse<AuthProvider>(result.Provider, true, out provider))
            {
                switch (provider)
                {
                    case AuthProvider.Facebook:
                    case AuthProvider.LinkedIn:
                    case AuthProvider.Microsoft:
                    case AuthProvider.Twitter:
                        {
                            string fullName;
                            if (result.ExtraData.TryGetValue("name", out fullName))
                            {
                                return fullName;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    case AuthProvider.Google:
                        {
                            string firstName, lastName;
                            if (result.ExtraData.TryGetValue("firstName", out firstName) && result.ExtraData.TryGetValue("lastName", out lastName))
                            {
                                return firstName + " " + lastName;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    case AuthProvider.Yahoo:
                        {
                            string fullName;
                            if (result.ExtraData.TryGetValue("fullName", out fullName))
                            {
                                return fullName;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    default:
                        return null;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Unknown provider: {0}", result.Provider);
                return null;
            }
        }
    }
}
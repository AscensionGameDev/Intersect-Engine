using System;
using System.Text.RegularExpressions;
using Intersect.Localization;

namespace Intersect_Client.Classes.Misc
{
    public static class FieldChecking
    {
        //Field Checking
        public const string MatchEmailPattern =
                @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$"
            ;

        public const string UsernamePattern = @"^[a-zA-Z0-9]{2,20}$";
        public const string PasswordPattern = @"^[a-zA-Z0-9]{4,20}$";

        public static bool IsEmail(string email)
        {
            if (email != null)
            {
                try
                {
                    return Regex.IsMatch(email, Strings.Get("regex", "email"));
                }
                catch (ArgumentException)
                {
                    return Regex.IsMatch(email, MatchEmailPattern);
                }
            }
            return false;
        }

        public static bool IsValidName(string name)
        {
            if (name != null)
            {
                try
                {
                    return Regex.IsMatch(name.Trim(), Strings.Get("regex", "username"));
                }
                catch (ArgumentException)
                {
                    return Regex.IsMatch(name.Trim(), UsernamePattern);
                }
            }
            return false;
        }

        public static bool IsValidPass(string pass)
        {
            if (pass != null)
            {
                try
                {
                    return Regex.IsMatch(pass.Trim(), Strings.Get("regex", "password"));
                }
                catch (ArgumentException)
                {
                    return Regex.IsMatch(pass.Trim(), PasswordPattern);
                }
            }
            return false;
        }
    }
}
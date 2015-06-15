using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Intersect_Client.Classes
{
    public static class FieldChecking
    {
        //Field Checking
        public const string MatchEmailPattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
          + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        public const string UsernamePattern = @"^[a-zA-Z0-9]{2,20}$";
        public const string PasswordPattern = @"^[a-zA-Z0-9]{4,20}$";
        public static bool IsEmail(string email)
        {
            if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
            return false;
        }
        public static bool IsValidName(string name)
        {
            if (name != null) return Regex.IsMatch(name.Trim(), UsernamePattern);
            return false;
        }
        public static bool IsValidPass(string name)
        {
            if (name != null) return Regex.IsMatch(name.Trim(), PasswordPattern);
            return false;
        }
    }
}

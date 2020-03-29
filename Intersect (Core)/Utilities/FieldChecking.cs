using System;
using System.Text.RegularExpressions;

namespace Intersect.Utilities
{

    public static class FieldChecking
    {

        //Field Checking
        public const string PATTERN_EMAIL_ADDRESS =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

        public const string PATTERN_PASSWORD = @"^[-_=\+`~!@#\$%\^&\*()\[\]{}\\|;\:'"",<\.>/\?a-zA-Z0-9]{4,64}$";

        public const string PATTERN_USERNAME = @"^[a-zA-Z0-9]{2,24}$";

        public static bool IsWellformedEmailAddress(string email, string emailRegex)
        {
            if (email == null)
            {
                return false;
            }

            try
            {
                var customPattern = emailRegex;
                if (string.IsNullOrEmpty(customPattern))
                {
                    customPattern = PATTERN_EMAIL_ADDRESS;
                }

                return Regex.IsMatch(email, customPattern);
            }
            catch (ArgumentException)
            {
                return Regex.IsMatch(email, PATTERN_EMAIL_ADDRESS);
            }
        }

        public static bool IsValidUsername(string username, string usernameRegex)
        {
            if (username == null)
            {
                return false;
            }

            try
            {
                var customPattern = usernameRegex;
                if (string.IsNullOrEmpty(customPattern))
                {
                    customPattern = PATTERN_USERNAME;
                }

                return Regex.IsMatch(username.Trim(), customPattern);
            }
            catch (ArgumentException)
            {
                return Regex.IsMatch(username.Trim(), PATTERN_USERNAME);
            }
        }

        public static bool IsValidPassword(string password, string passwordRegex)
        {
            if (password == null)
            {
                return false;
            }

            try
            {
                var customPattern = passwordRegex;
                if (string.IsNullOrEmpty(customPattern))
                {
                    customPattern = PATTERN_PASSWORD;
                }

                return Regex.IsMatch(password.Trim(), customPattern);
            }
            catch (ArgumentException)
            {
                return Regex.IsMatch(password.Trim(), PATTERN_PASSWORD);
            }
        }

    }

}

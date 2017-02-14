/*
    The MIT License (MIT)

    Copyright (c) 2015 JC Snider, Joe Bridges
  
    Website: http://ascensiongamedev.com
    Contact Email: admin@ascensiongamedev.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/

using System;
using System.Text.RegularExpressions;
using Intersect_Library.Localization;

namespace Intersect_Client.Classes.Misc
{
    public static class FieldChecking
    {
        //Field Checking
        public const string MatchEmailPattern = @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
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
                    return Regex.IsMatch(name.Trim(), UsernamePattern );
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

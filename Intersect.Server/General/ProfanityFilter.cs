using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

using Intersect.Logging;


namespace Intersect.Server.General
{

    /// <summary>
    /// A class used to filter profanity from strings.
    /// </summary>
    public static class ProfanityFilter
    {
        #region Private Properties
        private static List<string> regexFilters;

        private static List<string> filteredWords;
        #endregion

        #region Serialization
        public static bool Load()
        {
            var filepath = Path.Combine("resources", "filteredwords.json");

            filteredWords = new List<string>();

            // Really don't want two JsonSave() return points...
            // ReSharper disable once InvertIf
            if (File.Exists(filepath))
            {
                var json = File.ReadAllText(filepath, Encoding.UTF8);
                try
                {
                    filteredWords = JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
                    regexFilters = filteredWords.Select(word => ToRegexPattern(word)).ToList();
                }
                catch (Exception exception)
                {
                    Log.Error(exception);

                    return false;
                }
            }

            return Save();
        }

        public static bool Save()
        {
            try
            {
                var filepath = Path.Combine("resources", "filteredwords.json");
                Directory.CreateDirectory("resources");
                var json = JsonConvert.SerializeObject(filteredWords, Formatting.Indented);
                File.WriteAllText(filepath, json, Encoding.UTF8);

                return true;
            }
            catch (Exception exception)
            {
                Log.Error(exception);

                return false;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Filter the provided string with the provided list.
        /// </summary>
        /// <param name="dirtyString">The dirty string that is to be filtered by the profanity filter.</param>
        /// <returns>Returns a clean string after filtering the dirty string with the provided list.</returns>
        public static string FilterWords(string dirtyString)
        {
            // Do we have something to filter?
            if (string.IsNullOrWhiteSpace(dirtyString))
            {
                return dirtyString;
            }

            // Go through our list of words to filter and take them all out!
            var filteredString = dirtyString;
            foreach (var filter in regexFilters)
            {
                filteredString = Regex.Replace(filteredString, filter, StarCensoredMatch, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            }

            return filteredString;
        }

        /// <summary>
        /// Checks whether a string contains a word that would be filtered by the provided list.
        /// </summary>
        /// <param name="dirtyString">The dirty string that is to be filtered by the profanity filter.</param>
        /// <returns>Returns whether the string contains a filtered word or not.</returns>
        public static bool ContainsFilteredWords(string dirtyString)
        {
            // Do we have something to filter?
            if (string.IsNullOrWhiteSpace(dirtyString))
            {
                return false;
            }

            // If we filter our string, does it change?
            if (FilterWords(dirtyString) != dirtyString)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region Private Methods
        // Credit goes to https://github.com/jamesmontemagno for these two methods.
        // His Censorship class was an inspiration to this, but felt like it could be handled a little more efficiently for Intersect.
        static string StarCensoredMatch(Group m) => new string('*', m.Captures[0].Value.Length);

        static string ToRegexPattern(string wildcardSearch)
        {
            var regexPattern = Regex.Escape(wildcardSearch);

            regexPattern = regexPattern.Replace(@"\*", ".*?");
            regexPattern = regexPattern.Replace(@"\?", ".");

            if (regexPattern.StartsWith(".*?", StringComparison.Ordinal))
            {
                regexPattern = regexPattern.Substring(3);
                regexPattern = @"(^\b)*?" + regexPattern;
            }

            regexPattern = @"\b" + regexPattern + @"\b";

            return regexPattern;
        }
        #endregion
    }
}

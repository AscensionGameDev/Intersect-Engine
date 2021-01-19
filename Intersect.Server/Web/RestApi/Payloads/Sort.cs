using System;
using System.Collections.Generic;
using System.Linq;

namespace Intersect.Server.Web.RestApi.Payloads
{

    public struct Sort
    {

        public string By { get; set; }

        public SortDirection Direction { get; set; }

        public static Sort From(string sortBy, SortDirection sortDirection)
        {
            return new Sort
            {
                By = sortBy,
                Direction = sortDirection
            };
        }

        public static Sort[] From(string[] sortBy, SortDirection[] sortDirections)
        {
            var filteredBy =
                sortBy?.Where(by => !string.IsNullOrWhiteSpace(by)).Take(sortDirections?.Length ?? 0).ToList() ??
                new List<string>();

            var filteredDirections = sortDirections?.Take(filteredBy.Count).ToList() ?? new List<SortDirection>();

            return filteredBy.Select(
                    (by, index) => From(by ?? throw new InvalidOperationException(), filteredDirections[index])
                )
                .ToArray();
        }

    }

}

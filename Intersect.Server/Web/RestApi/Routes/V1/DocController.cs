using System;

using JetBrains.Annotations;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

using Intersect.Server.Web.RestApi.Attributes;

namespace Intersect.Server.Web.RestApi.Routes.V1
{

    [RoutePrefix("doc")]
    [ConfigurableAuthorize]
    public sealed class DocController : ApiController
    {

        private Collection<ApiDescription> mDescriptions;

        [NotNull]
        private IEnumerable<ApiDescription> Descriptions
        {
            get
            {
                if (mDescriptions == null)
                {
                    mDescriptions = ControllerContext?.Configuration?.Services.GetApiExplorer()?.ApiDescriptions;
                }

                return mDescriptions ?? new Collection<ApiDescription>();
            }
        }

        [Route("{*path}")]
        [HttpGet]
        public object Authorized(string path)
        {
            var segments = path?.Trim().Split('/') ?? new string[0];

            var pathSegments = new List<string>();
            var descriptions = Descriptions.OrderBy(description => description?.RelativePath)
                .Where(
                    description =>
                    {
                        if (string.IsNullOrWhiteSpace(description?.RelativePath))
                        {
                            return false;
                        }

                        if (Descriptions.Count() == 1)
                        {
                            return true;
                        }

                        if (!description.RelativePath.StartsWith(path ?? ""))
                        {
                            return false;
                        }

                        var descriptionSegments = description.RelativePath.Split('/');
                        if (Math.Max(segments.Length, 1) + 1 >= descriptionSegments.Length)
                        {
                            pathSegments.Add(description.RelativePath);
                            return true;
                        }

                        var partialDescriptionPath = "";
                        for (var segmentIndex = 0;
                            segmentIndex < descriptionSegments.Length && segmentIndex < Math.Max(segments.Length, 1) + 1;
                            ++segmentIndex)
                        {
                            if (!string.IsNullOrWhiteSpace(partialDescriptionPath))
                            {
                                partialDescriptionPath += '/';
                            }

                            partialDescriptionPath += descriptionSegments[segmentIndex];
                        }

                        pathSegments.Add(partialDescriptionPath);
                        return false;
                    }
                )
                .ToList();

            var displaySegments = pathSegments
                .GroupBy(pathSegment => pathSegment)
                .Select(
                    group =>
                    {
                        dynamic expando = new ExpandoObject();
                        expando.path = group?.FirstOrDefault();
                        expando.count = group?.Count();
                        return expando;
                    }
                )
                .ToList();

            switch (descriptions.Count)
            {
                case 0:
                    return pathSegments.Select(pathSegment => new {path = pathSegment});

                case 1:
                    return new[]
                    {
                        descriptions.First()?.ToJson()
                    };

                default:
                    var showDetail = descriptions.All(
                        description => description?.RelativePath?.IndexOf('/', path?.Length ?? 0) < 1
                    );

                    return displaySegments
                        .Concat(
                            descriptions.Select(
                                description => description?.ToJson(parameters: showDetail, documentation: showDetail)
                            )
                        )
                        .Where(displaySegment => displaySegment != null)
                        // TODO: Either scrap this or reinstate it.
                        //.GroupBy(displaySegment => displaySegment.path)
                        //.Select(
                        //    group => group.OrderByDescending(
                        //            member => ((IDictionary<string, object>) member)?.ContainsKey("method") ?? false
                        //                ? member.method
                        //                : null
                        //        )
                        //        .First()
                        //)
                        .OrderBy(displaySegment => displaySegment?.path)
                        .ToArray();
            }
        }

    }

    internal static class ApiExtensions
    {

        public static dynamic ToJson([NotNull] this HttpParameterDescriptor descriptor)
        {
            return new
            {
                type = descriptor.ParameterType?.FullName,
                @default = descriptor.DefaultValue
            };
        }

        public static dynamic ToJson([NotNull] this ApiParameterDescription description)
        {
            return new
            {
                name = description.Name,
                documentation = description.Documentation,
                source = description.Source.ToString(),
                descriptor = description.ParameterDescriptor?.ToJson()
            };
        }

        public static dynamic ToJson(
            [NotNull] this ApiDescription description,
            bool method = true,
            bool documentation = true,
            bool parameters = true
        )
        {
            dynamic json = new ExpandoObject();

            json.path = description.RelativePath;

            if (method)
            {
                json.method = description.HttpMethod?.Method;
            }

            if (documentation)
            {
                json.documentation = description.Documentation;
            }

            if (parameters)
            {
                json.parameters = description.ParameterDescriptions?.Select(parameter => parameter?.ToJson()) ??
                                  new object[0];
            }

            return json;
        }

    }

}

using System;
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
            var segments = path?.Trim().Split('/') ?? Array.Empty<string>();

            var pathSegments = new List<string>();
            var descriptions = Descriptions.OrderBy(description => description?.RelativePath)
                .Where(
                    description =>
                    {
                        if (typeof(DemoController) ==
                            description?.ActionDescriptor?.ControllerDescriptor?.ControllerType)
                        {
                            return false;
                        }

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
                        if (Math.Max(segments.Length, 1) + 1 >=
                            descriptionSegments.Length - (description.ParameterDescriptions?.Count ?? 0))
                        {
                            pathSegments.Add(description.RelativePath);

                            return true;
                        }

                        var partialDescriptionPath = "";
                        for (var segmentIndex = 0;
                            segmentIndex < descriptionSegments.Length &&
                            segmentIndex < Math.Max(segments.Length, 1) + 1;
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

            var displaySegments = pathSegments.GroupBy(pathSegment => pathSegment)
                .Select(
                    group =>
                    {
                        dynamic expando = new ExpandoObject();
                        expando.path = group?.FirstOrDefault();
                        expando.children = group?.Count();

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
                    // TODO: Make this not show detail if it has multiple differing branches (e.g. if looking at /api/v1/ don't show detail when there's /api/v1/doc/* and /api/v1/info/*)
                    var showDetail = descriptions.All(
                        description =>
                            description?.RelativePath?.Split('/').Length -
                            (description?.ParameterDescriptions?.Count + 1) <=
                            segments.Length
                    );

                    // TODO: ABSOLUTELY REWRITE THIS SO THIS STUFF IS SOMEHOW PRE-CALCULATED AND NOT USING DYNAMICS!!! I AM LOOKING AT YOU, ME, MR. AUTHOR OF THIS GARBAGE PILE. -panda
                    return displaySegments
                        .Concat(
                            descriptions.Select(
                                description => description?.ToJson(parameters: showDetail, documentation: showDetail)
                            )
                        )
                        .Where(displaySegment => displaySegment != null)
                        .GroupBy(displaySegment => displaySegment.path)
                        .SelectMany(
                            group => group.Select(
                                    member =>
                                    {
                                        var lookup = (IDictionary<string, object>) member;
                                        var score = 0;
                                        var hasMethod = false;

                                        if (lookup?.ContainsKey("method") ?? false)
                                        {
                                            hasMethod = true;
                                            score = 1;
                                        }
                                        else if (lookup?.ContainsKey("children") ?? false)
                                        {
                                            score = member.children;
                                        }

                                        return new
                                        {
                                            member,
                                            score,
                                            hasMethod
                                        };
                                    }
                                )
                                .GroupBy(compound => compound.score)
                                .Select(subgroup => subgroup.OrderByDescending(compound => compound.hasMethod).First())
                                .OrderBy(compound => compound?.score)
                        )
                        .Select(compound => compound.member)
                        .OrderBy(displaySegment => displaySegment?.path)
                        .ToArray();
            }
        }

    }

    internal static class ApiExtensions
    {

        public static dynamic ToJson(this HttpParameterDescriptor descriptor)
        {
            return new
            {
                type = descriptor.ParameterType?.FullName,
                @default = descriptor.DefaultValue
            };
        }

        public static dynamic ToJson(this ApiParameterDescription description)
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
            this ApiDescription description,
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
                                  Array.Empty<object>();
            }

            return json;
        }

    }

}

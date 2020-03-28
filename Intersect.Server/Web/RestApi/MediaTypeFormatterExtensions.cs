using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

using JetBrains.Annotations;

namespace Intersect.Server.Web.RestApi
{

    internal static class MediaTypeFormatterExtensions
    {

        [NotNull]
        public static MediaTypeFormatter AddSupportedMediaType(
            [NotNull] this MediaTypeFormatter mediaTypeFormatter,
            [NotNull] string mimeType
        )
        {
            var mediaTypeHeaderValue =
                mediaTypeFormatter.SupportedMediaTypes?.FirstOrDefault(FindMediaTypeHeaderValue(mimeType));

            if (mediaTypeHeaderValue == null)
            {
                mediaTypeFormatter.SupportedMediaTypes?.Add(new MediaTypeHeaderValue(mimeType));
            }

            return mediaTypeFormatter;
        }

        [NotNull]
        public static MediaTypeFormatter RemoveSupportedMediaType(
            [NotNull] this MediaTypeFormatter mediaTypeFormatter,
            [NotNull] string mimeType
        )
        {
            var mediaTypeHeaderValue =
                mediaTypeFormatter.SupportedMediaTypes?.FirstOrDefault(FindMediaTypeHeaderValue(mimeType));

            if (mediaTypeHeaderValue != null)
            {
                mediaTypeFormatter.SupportedMediaTypes?.Remove(mediaTypeHeaderValue);
            }

            return mediaTypeFormatter;
        }

        [NotNull]
        public static MediaTypeFormatter Map(
            [NotNull] this MediaTypeFormatter mediaTypeFormatter,
            [NotNull] string headerName,
            [NotNull] string headerValue,
            [NotNull] string mediaType,
            bool isValueSubstring = true,
            StringComparison valueComparison = StringComparison.OrdinalIgnoreCase
        )
        {
            mediaTypeFormatter.MediaTypeMappings?.Add(
                new RequestHeaderMapping(headerName, headerValue, valueComparison, isValueSubstring, mediaType)
            );

            return mediaTypeFormatter;
        }

        [NotNull]
        public static MediaTypeFormatter Unmap(
            [NotNull] this MediaTypeFormatter mediaTypeFormatter,
            [NotNull] string headerName,
            [NotNull] string headerValue,
            [CanBeNull] string mediaType = null
        )
        {
            var requestHeaderMapping =
                mediaTypeFormatter.MediaTypeMappings?.FirstOrDefault(
                    FindRequestHeaderMapping(headerName, headerValue, mediaType)
                );

            if (requestHeaderMapping != null)
            {
                mediaTypeFormatter.MediaTypeMappings?.Remove(requestHeaderMapping);
            }

            return mediaTypeFormatter;
        }

        [NotNull]
        public static Func<MediaTypeHeaderValue, bool> FindMediaTypeHeaderValue([NotNull] string mimeType)
        {
            return mediaTypeHeaderValue => string.Equals(
                mediaTypeHeaderValue?.MediaType, mimeType, StringComparison.OrdinalIgnoreCase
            );
        }

        [NotNull]
        public static Func<MediaTypeMapping, bool> FindRequestHeaderMapping(
            [NotNull] string headerName,
            [NotNull] string headerValue,
            [CanBeNull] string mediaType = null
        )
        {
            return mediaTypeMapping =>
            {
                if (!(mediaTypeMapping is RequestHeaderMapping requestHeaderMapping))
                {
                    return false;
                }

                if (!string.Equals(requestHeaderMapping?.HeaderName, headerName, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (!string.Equals(requestHeaderMapping?.HeaderValue, headerValue, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                return mediaType == null ||
                       string.Equals(
                           requestHeaderMapping?.MediaType?.MediaType, mediaType, StringComparison.OrdinalIgnoreCase
                       );
            };
        }

    }

}

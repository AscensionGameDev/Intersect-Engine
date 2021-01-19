using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace Intersect.Server.Web.RestApi
{

    internal static class MediaTypeFormatterExtensions
    {

        public static MediaTypeFormatter AddSupportedMediaType(
            this MediaTypeFormatter mediaTypeFormatter,
            string mimeType
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

        public static MediaTypeFormatter RemoveSupportedMediaType(
            this MediaTypeFormatter mediaTypeFormatter,
            string mimeType
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

        public static MediaTypeFormatter Map(
            this MediaTypeFormatter mediaTypeFormatter,
            string headerName,
            string headerValue,
            string mediaType,
            bool isValueSubstring = true,
            StringComparison valueComparison = StringComparison.OrdinalIgnoreCase
        )
        {
            mediaTypeFormatter.MediaTypeMappings?.Add(
                new RequestHeaderMapping(headerName, headerValue, valueComparison, isValueSubstring, mediaType)
            );

            return mediaTypeFormatter;
        }

        public static MediaTypeFormatter Unmap(
            this MediaTypeFormatter mediaTypeFormatter,
            string headerName,
            string headerValue,
            string mediaType = null
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

        public static Func<MediaTypeHeaderValue, bool> FindMediaTypeHeaderValue(string mimeType)
        {
            return mediaTypeHeaderValue => string.Equals(
                mediaTypeHeaderValue?.MediaType, mimeType, StringComparison.OrdinalIgnoreCase
            );
        }

        public static Func<MediaTypeMapping, bool> FindRequestHeaderMapping(
            string headerName,
            string headerValue,
            string mediaType = null
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

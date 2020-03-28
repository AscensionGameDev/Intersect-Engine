using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Intersect.Logging;
using Intersect.Utilities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Intersect.Server.Database.Logging
{

    [Serializable]
    public class RequestLog
    {

        private string mMethod;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 0)]
        [Key]
        public Guid Id { get; set; }

        public DateTime Time { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public LogLevel Level { get; set; }

        public string Method
        {
            get => mMethod;
            set => mMethod = value?.ToUpperInvariant();
        }

        public int StatusCode { get; set; }

        public string StatusMessage { get; set; }

        public string Uri { get; set; }

        private string SerializedRequestHeaders
        {
            get => JsonConvert.SerializeObject(RequestHeaders);
            set => RequestHeaders = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(value);
        }

        [NotMapped]
        public IDictionary<string, List<string>> RequestHeaders { get; set; }

        private string SerializedResponseHeaders
        {
            get => JsonConvert.SerializeObject(ResponseHeaders);
            set => ResponseHeaders = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(value);
        }

        [NotMapped]
        public IDictionary<string, List<string>> ResponseHeaders { get; set; }

        public class Mapper : IEntityTypeConfiguration<RequestLog>
        {

            /// <inheritdoc />
            public void Configure(EntityTypeBuilder<RequestLog> builder)
            {
                if (builder == null)
                {
                    throw new ArgumentNullException(nameof(builder));
                }

                builder.Property(rl => rl.SerializedRequestHeaders).IsNotNull().HasColumnName(nameof(RequestHeaders));

                builder.Property(rl => rl.SerializedResponseHeaders).IsNotNull().HasColumnName(nameof(ResponseHeaders));
            }

        }

    }

}

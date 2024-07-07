﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Intersect.Utilities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using LogLevel = Intersect.Logging.LogLevel;

namespace Intersect.Server.Database.Logging;

[Serializable]
public partial class RequestLog
{
    private string mMethod;

    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column(Order = 0)]
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

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

    public partial class Mapper : IEntityTypeConfiguration<RequestLog>
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

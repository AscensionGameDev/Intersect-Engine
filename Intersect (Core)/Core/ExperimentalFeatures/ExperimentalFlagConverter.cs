using System;

using Newtonsoft.Json;

namespace Intersect.Core.ExperimentalFeatures
{

    public class ExperimentalFlagConverter : JsonConverter<ExperimentalFlag>
    {

        public override bool CanRead => true;

        public override bool CanWrite => true;

        /// <inheritdoc />
        public override void WriteJson(
            JsonWriter writer,
            ExperimentalFlag value,
            JsonSerializer serializer
        )
        {
            serializer.Serialize(writer, value);
        }

        /// <inheritdoc />
        public override ExperimentalFlag ReadJson(
            JsonReader reader,
            Type objectType,
            ExperimentalFlag existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        )
        {
            serializer.Populate(reader, existingValue);

            return existingValue;
        }

    }

}

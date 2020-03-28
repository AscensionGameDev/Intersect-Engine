using System;

using JetBrains.Annotations;

using Newtonsoft.Json;

namespace Intersect.Core.ExperimentalFeatures
{

    public class ExperimentalFlagConverter : JsonConverter<ExperimentalFlag>
    {

        public override bool CanRead => true;

        public override bool CanWrite => true;

        /// <inheritdoc />
        public override void WriteJson(
            [NotNull] JsonWriter writer,
            ExperimentalFlag value,
            [NotNull] JsonSerializer serializer
        )
        {
            serializer.Serialize(writer, value);
        }

        /// <inheritdoc />
        public override ExperimentalFlag ReadJson(
            [NotNull] JsonReader reader,
            [NotNull] Type objectType,
            ExperimentalFlag existingValue,
            bool hasExistingValue,
            [NotNull] JsonSerializer serializer
        )
        {
            serializer.Populate(reader, existingValue);

            return existingValue;
        }

    }

}

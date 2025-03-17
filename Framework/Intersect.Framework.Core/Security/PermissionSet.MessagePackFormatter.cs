using MessagePack;
using MessagePack.Formatters;

namespace Intersect.Framework.Core.Security;

public sealed partial class PermissionSet
{
    public sealed partial class PermissionSetMessagePackFormatter : IMessagePackFormatter<PermissionSet>
    {
        public void Serialize(ref MessagePackWriter writer, PermissionSet? value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            if (string.IsNullOrWhiteSpace(value.Name))
            {
                throw new ArgumentException($"The {nameof(PermissionSet)} {nameof(Name)} must be non-empty");
            }

            var permissionSetName = value.Name;
            writer.Write(permissionSetName);

            writer.Write(value._inheritsFrom?.Name ?? value._inheritsFromName);

            var permissionGranted = value._permissionGranted;
            var count = permissionGranted.Count;
            var writtenCount = 0;
            writer.WriteMapHeader(count);
            foreach (var (permission, granted) in permissionGranted)
            {
                ++writtenCount;
                writer.Write(permission.Name);
                writer.Write(granted);
            }

            if (writtenCount != count)
            {
                throw new MessagePackSerializationException(
                    $"There are supposed to be {count} permissions in the {nameof(PermissionSet)} '{permissionSetName}' but {writtenCount} were written. It appears the set was modified while being serialized."
                );
            }
        }

        public PermissionSet? Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            var name = reader.ReadString();
            var inheritsFromName = reader.ReadString();

            if (!reader.TryReadMapHeader(out var permissionCount))
            {
                throw new MessagePackSerializationException($"Failed to read {nameof(PermissionSet)} map header.");
            }

            PermissionSet permissionSet = new(name: name, inheritsFromName: inheritsFromName);
            permissionSet._permissionGranted.EnsureCapacity(permissionCount);
            while (permissionSet._permissionGranted.Count < permissionCount)
            {
                var permissionName = reader.ReadString();
                if (permissionName == null)
                {
                    throw new MessagePackSerializationException(
                        $"The name of {permissionSet._permissionGranted.Count} is null, this isn't a valid state"
                    );
                }

                var permissionGranted = reader.ReadBoolean();

                Permission permission = permissionName;
                permissionSet._permissionGranted[permission] = permissionGranted;
            }

            UpdatePermissionSet(permissionSet);

            return permissionSet;
        }
    }
}
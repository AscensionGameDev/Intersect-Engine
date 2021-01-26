using Intersect.Logging;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Network
{
    public class MessagePacker
    {
        public static readonly MessagePacker Instance = new MessagePacker();

        private readonly MessagePackSerializerOptions mOptions  = MessagePackSerializerOptions.Standard.WithCompression(MessagePack.MessagePackCompression.Lz4BlockArray).
            WithResolver(MessagePack.Resolvers.CompositeResolver.Create(
                new[] { 
                    MessagePack.Formatters.TypelessFormatter.Instance 
                }, 
                new IFormatterResolver[] { 
                    MessagePack.Resolvers.NativeGuidResolver.Instance, 
                    MessagePack.Resolvers.NativeDateTimeResolver.Instance, 
                    MessagePack.Resolvers.NativeDecimalResolver.Instance, 
                    MessagePack.Resolvers.StandardResolver.Instance }
                )
            );

        public byte[] Serialize(object obj)
        {
            return MessagePackSerializer.Typeless.Serialize(obj, mOptions);
        }

        public object Deserialize(byte[] data)
        {
            try
            {
                return MessagePackSerializer.Typeless.Deserialize(data, mOptions);
            }
            catch (Exception exception)
            {
                Log.Error(exception);

                return null;
            }
        }

    }
}

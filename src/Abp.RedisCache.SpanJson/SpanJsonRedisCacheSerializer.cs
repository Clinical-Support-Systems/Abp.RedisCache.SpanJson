using SpanJson;
using SpanJson.Resolvers;
using StackExchange.Redis;
using System;
using System.Buffers;

namespace Abp.Runtime.Caching.Redis
{
    /// <inheritdoc />
    public class SpanJsonRedisCacheSerializer : DefaultRedisCacheSerializer
    {
        private const string SpanJsonPrefix = "SJ^";
        private const string TypeSeperator = "|";

        /// <inheritdoc />
        public override object Deserialize(RedisValue objbyte)
        {
            string serializedObj = objbyte;
            if (!serializedObj.StartsWith(SpanJsonPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return base.Deserialize(objbyte);
            }

            serializedObj = serializedObj[SpanJsonPrefix.Length..];
            var typeSeperatorIndex = serializedObj.IndexOf(TypeSeperator, StringComparison.OrdinalIgnoreCase);
            var type = Type.GetType(serializedObj[..typeSeperatorIndex]);
            var serialized = serializedObj[(typeSeperatorIndex + 1)..];
            var byteAfter64 = Convert.FromBase64String(serialized);

            return JsonSerializer.NonGeneric.Utf8.Deserialize(byteAfter64.AsSpan(), type);
        }

        /// <inheritdoc />
        public override RedisValue Serialize(object value, Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var serialized = JsonSerializer.NonGeneric.Utf8.SerializeToArrayPool<IncludeNullsOriginalCaseResolver<byte>>(value);
            try
            {
                var minified = JsonSerializer.Minifier.Minify(serialized);
                var serializedContent = Convert.ToBase64String(minified, 0, minified.Length);
                return $"{SpanJsonPrefix}{type.AssemblyQualifiedName}{TypeSeperator}{serializedContent}";
            }
            finally
            {
                if (serialized.Array != null)
                    ArrayPool<byte>.Shared.Return(serialized.Array);
            }
        }
    }
}
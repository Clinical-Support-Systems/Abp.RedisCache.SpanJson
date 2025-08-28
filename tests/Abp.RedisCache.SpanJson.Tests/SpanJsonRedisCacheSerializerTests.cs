using System.Text;
using System.Text.Json;
using Abp.Runtime.Caching.Redis;
using Shouldly;
using SpanJson;
using StackExchange.Redis;
using Xunit.Abstractions;

namespace Abp.RedisCache.SpanJson.Tests
{
    public class SpanJsonRedisCacheSerializerTests : TestBaseWithLocalIocManager
    {
        public SpanJsonRedisCacheSerializerTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Simple_Serialize_Deserialize_Test()
        {
            //Arrange
            var spanJsonSerailizer = new SpanJsonRedisCacheSerializer();
            var objectToSerialize = new ClassToSerialize
            {
                Age = 10,
                Name = Guid.NewGuid().ToString(),
                DateOfBirth = DateTime.Now
            };

            //Act
            var classSerializedString = spanJsonSerailizer.Serialize(
                objectToSerialize,
                typeof(ClassToSerialize)
            );

            Output.WriteLine(classSerializedString);

            var classUnSerialized = spanJsonSerailizer.Deserialize(classSerializedString);

            //Assert
            classUnSerialized.ShouldBeOfType<ClassToSerialize>();
            var classUnSerializedTyped = (ClassToSerialize)classUnSerialized;
            classUnSerializedTyped.Age.ShouldBe(objectToSerialize.Age);
            classUnSerializedTyped.Name.ShouldBe(objectToSerialize.Name);
            classUnSerializedTyped.DateOfBirth.ShouldBe(objectToSerialize.DateOfBirth);
        }

        [Fact]
        public void Serialize_NullType_Throws_ArgumentNullException()
        {
            // Arrange
            var serializer = new SpanJsonRedisCacheSerializer();
            var obj = new Sample
            {
                Age = 1,
                Name = "A",
                DateOfBirth = DateTime.UnixEpoch
            };

            // Act + Assert
            Should.Throw<ArgumentNullException>(() => serializer.Serialize(obj, null!));
        }

        [Fact]
        public void Serialize_Produces_Prefixed_TypeAnnotated_Minified_Base64_Payload()
        {
            // Arrange
            var serializer = new SpanJsonRedisCacheSerializer();
            var obj = new Sample
            {
                Age = 42,
                Name = "John Doe",
                DateOfBirth = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc)
            };

            // Act
            var rv = serializer.Serialize(obj, typeof(Sample));
            rv.IsNull.ShouldBeFalse();

            var s = (string?)rv;
            s.ShouldNotBeNull();

            // Assert: prefix and separator
            s.StartsWith("SJ^").ShouldBeTrue();
            var sepIndex = s.IndexOf('|');
            sepIndex.ShouldBeGreaterThan(3);

            var typePart = s.Substring(3, sepIndex - 3);
            typePart.ShouldBe(typeof(Sample).AssemblyQualifiedName);

            var base64 = s[(sepIndex + 1)..];
            base64.ShouldNotBeNullOrWhiteSpace();

            // Decode payload and verify it is minified JSON with original-case property names
            var jsonBytes = Convert.FromBase64String(base64);
            var json = Encoding.UTF8.GetString(jsonBytes);
            Output.WriteLine(json);

            json.ShouldContain("\"Age\"");
            json.ShouldContain("\"Name\"");
            json.ShouldContain("\"DateOfBirth\"");
            // Minified => should not contain spaces or newlines (allow spaces inside string if any, but this is a simple check)
            json.ShouldNotContain("\n");
            json.ShouldNotContain("  ");
        }

        [Fact]
        public void Deserialize_Null_Or_Empty_Returns_Null()
        {
            var serializer = new SpanJsonRedisCacheSerializer();

            serializer.Deserialize(RedisValue.Null).ShouldBeNull();
            serializer.Deserialize(RedisValue.EmptyString).ShouldBeNull();
        }

        [Fact]
        public void Deserialize_From_NonString_RedisValue_Delegates_To_Base_And_May_Throw()
        {
            var serializer = new SpanJsonRedisCacheSerializer();
            var rv = (RedisValue)new byte[] { 1, 2, 3, 4 };

            // Since payload does not start with our prefix, it goes to base.Deserialize
            // which will try System.Text.Json and throw for invalid JSON payloads.
            Should.Throw<JsonException>(() => serializer.Deserialize(rv));
        }

        [Fact]
        public void Deserialize_With_Corrupted_Base64_Throws_FormatException()
        {
            var serializer = new SpanJsonRedisCacheSerializer();
            var corrupt = $"SJ^{typeof(Sample).AssemblyQualifiedName}|not-a-base64";

            Should.Throw<FormatException>(() => serializer.Deserialize(corrupt));
        }

        [Fact]
        public void Deserialize_With_Unknown_Type_Throws_ArgumentNullException()
        {
            var serializer = new SpanJsonRedisCacheSerializer();

            // Prepare a valid base64 of some json payload
            const string json = "{\"Age\":1,\"Name\":\"X\",\"DOB\":\"2000-01-01T00:00:00Z\"}";
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            const string unknownTypeName = "System.ThisType.Does.Not.Exist, mscorlib";
            var payload = $"SJ^{unknownTypeName}|{base64}";

            Should.Throw<ArgumentNullException>(() => serializer.Deserialize(payload));
        }

        [Fact]
        public void Roundtrip_Supported_Type_Works()
        {
            var serializer = new SpanJsonRedisCacheSerializer();
            var obj = new Sample
            {
                Age = 77,
                Name = Guid.NewGuid().ToString(),
                DateOfBirth = DateTime.UtcNow
            };

            var rv = serializer.Serialize(obj, typeof(Sample));
            var deserialized = serializer.Deserialize(rv);

            deserialized.ShouldBeOfType<Sample>();
            var typed = (Sample)deserialized!;
            typed.Age.ShouldBe(obj.Age);
            typed.Name.ShouldBe(obj.Name);
            typed.DateOfBirth.ShouldBe(obj.DateOfBirth);
        }

        private class ClassToSerialize
        {
            [JsonConstructor(nameof(Age), nameof(Name), nameof(DateOfBirth))]
            public ClassToSerialize(int age, string name, DateTime dateOfBirth)
            {
                // This constructor gets called when I'm deserializing
                Age = age;
                Name = name;
                DateOfBirth = dateOfBirth;
            }

            public ClassToSerialize()
            {
                // This gets called when I make the object
            }

            public int Age { get; init; }

            public string? Name { get; init; }

            public DateTime DateOfBirth { get; init; }
        }

        private sealed class Sample
        {
            public int Age { get; init; }
            public string? Name { get; init; }
            public DateTime DateOfBirth { get; init; }
        }
    }
}

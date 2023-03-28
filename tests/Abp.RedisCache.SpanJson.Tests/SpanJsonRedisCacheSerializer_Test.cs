using Abp.Runtime.Caching.Redis;
using Shouldly;
using SpanJson;
using Xunit.Abstractions;

namespace Abp.RedisCache.SpanJson.Tests
{
    public class SpanJsonRedisCacheSerializer_Test : TestBaseWithLocalIocManager
    {
        public SpanJsonRedisCacheSerializer_Test(ITestOutputHelper output) : base(output)
        {
        }

        public class ClassToSerialize
        {
            [JsonConstructor(nameof(Age), nameof(Name), nameof(DOB))]
            public ClassToSerialize(int age, string name, DateTime dob)
            {
                // This constructor gets called when I'm deserializing
                Age = age;
                Name = name;
                DOB = dob;
            }

            public ClassToSerialize()
            {
                // This gets called when I make the object
            }

            public int Age { get; set; }

            public string? Name { get; set; }

            public DateTime DOB { get; set; }
        }

        [Fact]
        public void Simple_Serialize_Deserialize_Test()
        {
            //Arrange
            var spanJsonSerailizer = new SpanJsonRedisCacheSerializer();
            var objectToSerialize = new ClassToSerialize { Age = 10, Name = Guid.NewGuid().ToString(), DOB = DateTime.Now };

            //Act
            string classSerializedString = spanJsonSerailizer.Serialize(
                objectToSerialize,
                typeof(ClassToSerialize)
            );

            Output.WriteLine(classSerializedString);

            object classUnSerialized = spanJsonSerailizer.Deserialize(classSerializedString);

            //Assert
            classUnSerialized.ShouldBeOfType<ClassToSerialize>();
            ClassToSerialize classUnSerializedTyped = (ClassToSerialize)classUnSerialized;
            classUnSerializedTyped.Age.ShouldBe(objectToSerialize.Age);
            classUnSerializedTyped.Name.ShouldBe(objectToSerialize.Name);
            classUnSerializedTyped.DOB.ShouldBe(objectToSerialize.DOB);
        }
    }
}
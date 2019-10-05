using Newtonsoft.Json;
using Xunit.Abstractions;

namespace ApplicationCore.Tests
{
    /// <summary>
    /// Allows Theory's single test case to be debugged through Test Explorer.
    /// Each test case wrapped in this class will be shown in the Test Explorer window under the test class.
    /// See more from https://stackoverflow.com/a/43068778/1646905 and https://github.com/xunit/xunit/issues/649
    /// </summary>
    /// <typeparam name="TCase">Test case data type</typeparam>
    public class MemberDataSerializer<TCase> : IXunitSerializable
    {
        /// <summary>
        /// Test case description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Test case data
        /// </summary>
        public TCase Object { get; private set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        /// <summary>
        /// Constructor to comply with XUnit serialization requirements
        /// </summary>
        public MemberDataSerializer() { }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        public MemberDataSerializer(string description, TCase objectToSerialize)
        {
            Description = description;
            Object = objectToSerialize;
        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            Object = JsonConvert.DeserializeObject<TCase>(info.GetValue<string>("objValue"));
            Description = JsonConvert.DeserializeObject<string>(info.GetValue<string>("description"));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            var json = JsonConvert.SerializeObject(Object);
            info.AddValue("objValue", json);
            var descJson = JsonConvert.SerializeObject(Description);
            info.AddValue("description", descJson);
        }
    }
}

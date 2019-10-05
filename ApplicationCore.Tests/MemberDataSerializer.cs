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
    public class MemberDataSerializer<TCase> : IXunitSerializable where TCase : class
    {
        /// <summary>
        /// Test case description. This is automatically set by xUnit.
        /// </summary>
        public string Description { get; private set; } = null!; // null-forgiving operator, because this can't be null

        /// <summary>
        /// Test case data. This is automatically set by xUnit.
        /// </summary>
        public TCase TestCase { get; private set; } = null!; // null-forgiving operator, because this can't be null

        /// <summary>
        /// Constructor to comply with xUnit serialization requirements
        /// </summary>
        public MemberDataSerializer() { }

        public MemberDataSerializer(string description, TCase objectToSerialize)
        {
            Description = description;
            TestCase = objectToSerialize;
        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            if (info is null)
            {
                throw new System.ArgumentNullException(nameof(info));
            }

            TestCase = JsonConvert.DeserializeObject<TCase>(info.GetValue<string>("testCaseValue"));
            Description = JsonConvert.DeserializeObject<string>(info.GetValue<string>("description"));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            if (info is null)
            {
                throw new System.ArgumentNullException(nameof(info));
            }

            var json = JsonConvert.SerializeObject(TestCase);
            info.AddValue("testCaseValue", json);
            var descJson = JsonConvert.SerializeObject(Description);
            info.AddValue("description", descJson);
        }
    }
}

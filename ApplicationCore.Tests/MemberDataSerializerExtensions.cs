using System;
using System.Collections.Generic;

namespace ApplicationCore.Tests
{
    public static class MemberDataSerializerExtensions
    {
        /// <summary>
        /// Wraps <see cref="FormTestCase"/> to <see cref="MemberDataSerializer{TCase}"/> so that single Theory
        /// test case can debugged separately
        /// </summary>
        /// <param name="testCase">Test case to wrap</param>
        /// <param name="description">Short and easy to read test case description</param>
        /// <returns>Wrapped <see cref="FormTestCase"/></returns>
        public static MemberDataSerializer<IEnumerable<KeyValuePair<string, string>>> ForTestCase(
            this IEnumerable<KeyValuePair<string, string>> testCase,
            string description)
        {
            if (testCase is null)
            {
                throw new ArgumentNullException(nameof(testCase));
            }

            return new MemberDataSerializer<IEnumerable<KeyValuePair<string, string>>>(description, testCase);
        }
    }
}

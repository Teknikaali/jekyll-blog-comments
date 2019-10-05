using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using ApplicationCore.Model;
using Xunit;

namespace ApplicationCore.Tests
{
    public class CommentFormTests
    {
        [Theory]
        [MemberData(nameof(ValidFormValues))]
        public void ValidFormShouldNotHaveErrors(MemberDataSerializer<IEnumerable<KeyValuePair<string, string>>> testCase)
        {
            var form = new NameValueCollection();
            foreach (var fieldValuePair in testCase.TestCase)
            {
                form.Add(fieldValuePair.Key, fieldValuePair.Value);
            }

            var commentForm = new CommentForm(form);

            Assert.False(
                commentForm.HasErrors,
                $"Comment form should not have errors. Errors: {FormatErrors(commentForm.Errors)}");
        }

        [Theory]
        [MemberData(nameof(InvalidFormValues))]
        public void InvalidFormShouldHaveErrors(MemberDataSerializer<IEnumerable<KeyValuePair<string, string>>> testCase)
        {
            var form = new NameValueCollection();
            foreach (var fieldValuePair in testCase.TestCase)
            {
                form.Add(fieldValuePair.Key, fieldValuePair.Value);
            }

            var commentForm = new CommentForm(form);

            Assert.True(
                commentForm.HasErrors,
                $"Comment form should have errors. Errors: {FormatErrors(commentForm.Errors)}");
        }

        public static IEnumerable<object[]> ValidFormValues()
        {
            yield return new MemberDataSerializer<IEnumerable<KeyValuePair<string, string>>>[]
            {
                new List<KeyValuePair<string, string>>
                {
                    CreateField("postId", "this-is-a-post-slug"),
                    CreateField("message", "This is the message"),
                    CreateField("name", "My Name")
                }.ForTestCase("All required fields and no optionals")
            };

            yield return new MemberDataSerializer<IEnumerable<KeyValuePair<string, string>>>[]
            {
                new List<KeyValuePair<string, string>>
                {
                    CreateField("postId", "this-is-a-post-slug"),
                    CreateField("message", "This is the message"),
                    CreateField("name", "My Very Own Name"),
                    CreateField("email", "foo@example.com")
                }.ForTestCase("All required fields and 1 optional")
            };

            yield return new MemberDataSerializer<IEnumerable<KeyValuePair<string, string>>>[]
            {
                new List<KeyValuePair<string, string>>
                {
                    CreateField("postId", "this-is-a-post-slug"),
                    CreateField("message", "This is the message"),
                    CreateField("name", "My Very Own Name"),
                    CreateField("email", "foo@example.com"),
                    CreateField("url", "http://www.example.com"),
                    CreateField("avatar", "http://www.example.com/avatar.png"),
                    CreateField("score", "0.5")
                }.ForTestCase("All required fields and all optionals")
            };

            yield return new MemberDataSerializer<IEnumerable<KeyValuePair<string, string>>>[]
            {
                new List<KeyValuePair<string, string>>
                {
                    CreateField("postId", "this-is-a-post-slug"),
                    CreateField("message", "This is the message"),
                    CreateField("name", "My Very Own Name"),
                    CreateField("email", "foo@example.com"),
                    CreateField("url", string.Empty),
                    CreateField("avatar", string.Empty),
                    CreateField("score", string.Empty)
                }.ForTestCase("All required fields, 1 optional and many empty optionals")
            };

            yield return new MemberDataSerializer<IEnumerable<KeyValuePair<string, string>>>[]
            {
                new List<KeyValuePair<string, string>>
                {
                    CreateField("score", "0.5"),
                    CreateField("avatar", "http://www.example.com/avatar.png"),
                    CreateField("url", "http://www.example.com"),
                    CreateField("email", "foo@example.com"),
                    CreateField("name", "My Very Own Name"),
                    CreateField("message", "This is the message"),
                    CreateField("postId", "this-is-a-post-slug")
                }.ForTestCase("All fields in reverse order")
            };
            yield return new MemberDataSerializer<IEnumerable<KeyValuePair<string, string>>>[]
            {
                new List<KeyValuePair<string, string>>
                {
                    CreateField("postId", "this-is-a-post-slug"),
                    CreateField("message", "This is the message"),
                    CreateField("name", "My Very Own Name"),
                    CreateField("url", "this-is-invalid-url")
                }.ForTestCase("Valid required fields with invalid url value")
            };
            yield return new MemberDataSerializer<IEnumerable<KeyValuePair<string, string>>>[]
            {
                new List<KeyValuePair<string, string>>
                {
                    CreateField("postId", "this-is-a-post-slug"),
                    CreateField("message", "This is the message"),
                    CreateField("name", "My Very Own Name"),
                    CreateField("thisIsInvalidKey", "this-should-be-okay")
                }.ForTestCase("Valid required fields with single invalid field")
            };
        }

        public static IEnumerable<object[]> InvalidFormValues()
        {
            yield return new MemberDataSerializer<IEnumerable<KeyValuePair<string, string>>>[]
            {
                new List<KeyValuePair<string, string>>().ForTestCase("No fields")
            };
            yield return new MemberDataSerializer<IEnumerable<KeyValuePair<string, string>>>[]
            {
                new List<KeyValuePair<string, string>>
                {
                    CreateField("postId", "this-is-a-post-slug")
                }.ForTestCase("Single valid required field")
            };
            yield return new MemberDataSerializer<IEnumerable<KeyValuePair<string, string>>>[]
            {
                new List<KeyValuePair<string, string>>
                {
                    CreateField("thisIsInvalidKey", "this-should-not-work")
                }.ForTestCase("Single invalid field")
            };
        }

        private static KeyValuePair<string, string> CreateField(string name, string value)
            => new KeyValuePair<string, string>(name, value);

        private static string FormatErrors(IEnumerable<string> errors)
            => string.Join(", ", errors);
    }

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

using System.Collections.Generic;
using System.Collections.Specialized;
using Xunit;

namespace ApplicationCore.Tests
{
    public class CommentFormTests
    {

        [Theory]
        [MemberData(nameof(ValidFormValues))]
        public void CommentFormCanBeCreatedWithValidValues(IEnumerable<KeyValuePair<string, string>> formValues)
        {
            var form = new NameValueCollection();
            foreach (var fieldValuePair in formValues)
            {
                form.Add(fieldValuePair.Key, fieldValuePair.Value);
            }

            var commentForm = new CommentForm(form);

            Assert.True(
                commentForm.IsValid,
                $"Comment form should not have errors. Errors: {FormatErrors(commentForm.Errors)}");
        }

        [Theory]
        [MemberData(nameof(InvalidFormValues))]
        public void CommentFormCantBeCreatedWithInvalidValues(IEnumerable<KeyValuePair<string, string>> formValues)
        {
            var form = new NameValueCollection();
            foreach (var fieldValuePair in formValues)
            {
                form.Add(fieldValuePair.Key, fieldValuePair.Value);
            }

            var commentForm = new CommentForm(form);

            Assert.False(
                commentForm.IsValid,
                $"Comment form should have errors. Errors: {FormatErrors(commentForm.Errors)}");
        }

        public static IEnumerable<object[]> ValidFormValues =>
            new List<object[]>
            {
                // All required fields + no optionals
                new object[]
                {
                    new List<KeyValuePair<string, string>>
                    {
                        CreateField("postId", "this-is-a-post-slug"),
                        CreateField("message", "This is the message"),
                        CreateField("name", "My Name")
                    }
                },
                // Required fields + 1 optional
                new object[]
                {
                    new List<KeyValuePair<string, string>>
                    {
                        CreateField("postId", "this-is-a-post-slug"),
                        CreateField("message", "This is the message"),
                        CreateField("name", "My Very Own Name"),
                        CreateField("email", "foo@example.com")
                    }
                },
                // Required fields + many optionals
                new object[]
                {
                    new List<KeyValuePair<string, string>>
                    {
                        CreateField("postId", "this-is-a-post-slug"),
                        CreateField("message", "This is the message"),
                        CreateField("name", "My Very Own Name"),
                        CreateField("email", "foo@example.com"),
                        CreateField("url", "www.example.com"),
                        CreateField("avatar", "www.example.com/avatar.png"),
                        CreateField("score", "0.5")
                    }
                },
                // Required fields + 1 optional + many empty optionals
                new object[]
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
                    }
                },
                // All fields in reverse order
                new object[]
                {
                    new List<KeyValuePair<string, string>>
                    {
                        CreateField("score", "0.5"),
                        CreateField("avatar", "www.example.com/avatar.png"),
                        CreateField("url", "www.example.com"),
                        CreateField("email", "foo@example.com"),
                        CreateField("name", "My Very Own Name"),
                        CreateField("message", "This is the message"),
                        CreateField("postId", "this-is-a-post-slug")
                    }
                }
            };

        public static IEnumerable<object[]> InvalidFormValues =>
            new List<object[]>
            {
                // No fields
                new object[]
                {
                    new List<KeyValuePair<string, string>>()
                },
                // Single valid required field
                new object[]
                {
                    new List<KeyValuePair<string, string>>
                    {
                        CreateField("postId", "this-is-a-post-slug")
                    }
                },
                // Single invalid field
                new object[]
                {
                    new List<KeyValuePair<string, string>>
                    {
                        CreateField("thisIsInvalidKey", "this-should-not-work")
                    }
                },
                // Valid required fields with single invalid field
                new object[]
                {
                    new List<KeyValuePair<string, string>>
                    {
                        CreateField("postId", "this-is-a-post-slug"),
                        CreateField("message", "This is the message"),
                        CreateField("name", "My Very Own Name"),
                        CreateField("thisIsInvalidKey", "this-should-not-work")
                    }
                },
                // Valid required fields + invalid url value
                new object[]
                {
                    new List<KeyValuePair<string, string>>
                    {
                        CreateField("postId", "this-is-a-post-slug"),
                        CreateField("message", "This is the message"),
                        CreateField("name", "My Very Own Name"),
                        CreateField("url", "this-is-invalid-url")
                    }
                }
            };

        private static KeyValuePair<string, string> CreateField(string name, string value)
            => new KeyValuePair<string, string>(name, value);

        private static string FormatErrors(IEnumerable<string> errors)
            => string.Join(", ", errors);
    }
}

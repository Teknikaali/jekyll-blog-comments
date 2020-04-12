using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore.Model
{
    public class CommentForm : ICommentForm
    {
        /// <summary>
        /// Simplest form of email validation
        /// </summary>
        private static readonly Regex _validEmail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        private readonly Dictionary<string, FormField> _fields;
        private readonly ConstructorInfo _constructor;

        public IEnumerable<string> Errors { get; }

        public bool HasErrors => Errors.Any();

        public CommentForm(IFormCollection form)
        {
            _constructor = typeof(Comment).GetConstructors()[0];

            _fields = _constructor.GetParameters().ToDictionary(
                p => p.Name!,
                p => TryConvertFormFieldValue(form[p.Name], p));

            var errors = _fields
                .Where(p => p.Value.HasError)
                .Select(p => p.Value.Error)
                .ToList();

            if (!IsEmailValid())
            {
                errors.Add("email is not in the correct format");
            }

            Errors = errors;
        }

        /// <summary>
        /// Try to create a Comment from the form.  Each Comment constructor argument will be name-matched
        /// against values in the form. Each non-optional arguments (those that don't have a default value)
        /// not supplied will cause an error in the list of errors and prevent the Comment from being created.
        /// </summary>
        public CommentResult TryCreateComment()
        {
            if (!Errors.Any())
            {
                return new CommentResult((Comment)_constructor.Invoke(_fields.Values.Select(x => x.Value).ToArray()));
            }
            else
            {
                return new CommentResult(
                    new Comment(string.Empty, string.Empty, string.Empty),
                    Errors,
                    new InvalidOperationException(
                        $"Couldn't create comment result. Reasons: {string.Join(',', Errors)}"));
            }
        }

        private static FormField TryConvertFormFieldValue(string fieldValue, ParameterInfo parameterInfo)
        {
            if (IsRequiredField())
            {
                return new FormField(TypeDescriptor.GetConverter(parameterInfo.ParameterType).ConvertFrom(fieldValue));
            }
            else if (IsOptionalField())
            {
                if (!string.IsNullOrEmpty(fieldValue))
                {
                    var converter = TypeDescriptor.GetConverter(parameterInfo.ParameterType);

                    if (converter.IsValid(fieldValue))
                    {
                        var convertedValue = converter.ConvertFrom(fieldValue);
                        return new FormField(convertedValue);
                    }
                    else
                    {
                        return new FormField(string.Format(
                            CultureInfo.InvariantCulture,
                            CommentResources.InvalidTypeConversionErrorMessage,
                            fieldValue,
                            parameterInfo.ParameterType.ToString()));
                    }
                }
                else
                {
                    return new FormField(parameterInfo.DefaultValue);
                }
            }
            else
            {
                // Field is required but missing a value
                return new FormField(string.Format(
                    CultureInfo.InvariantCulture,
                    CommentResources.MissingRequiredValueErrorMessage,
                    parameterInfo.Name));
            }

            bool IsRequiredField() => !string.IsNullOrWhiteSpace(fieldValue) && !parameterInfo.HasDefaultValue;
            bool IsOptionalField() => parameterInfo.HasDefaultValue;
        }

        // TODO: Remove magic string "email"
        private bool IsEmailValid()
        {
            var email = _fields["email"].Value as string;

            if (string.IsNullOrEmpty(email))
            {
                return true;
            }

            return _validEmail.IsMatch(email);
        }
    }
}

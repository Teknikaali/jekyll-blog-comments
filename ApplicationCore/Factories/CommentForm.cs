using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ApplicationCore.Model;

namespace ApplicationCore
{
    public class CommentForm
    {
        /// <summary>
        /// Simplest form of email validation
        /// </summary>
        private static readonly Regex _validEmail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        public Dictionary<string, FormField> Fields { get; }

        public IEnumerable<string> Errors { get; }

        public bool IsValid => !Errors.Any();

        private readonly ConstructorInfo _constructor;

        public CommentForm(NameValueCollection form)
        {
            _constructor = typeof(Comment).GetConstructors()[0];

            Fields = _constructor.GetParameters().ToDictionary(
                p => p.Name ?? throw new NullReferenceException("Constructor parameter name was null."),
                p => TryConvertFormFieldValue(form[p.Name], p));

            var errors = Fields
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
                return new CommentResult((Comment)_constructor.Invoke(Fields.Values.Select(x => x.Value).ToArray()));
            }
            else
            {
                return new CommentResult(new Comment(string.Empty, string.Empty, string.Empty), Errors);
            }
        }

        private static FormField TryConvertFormFieldValue(string fieldValue, ParameterInfo parameterInfo)
        {
            if (IsRequiredField())
            {
                return new FormField(
                    parameterInfo.Name,
                    TypeDescriptor.GetConverter(parameterInfo.ParameterType).ConvertFrom(fieldValue),
                    isRequired: true);
            }
            else if (IsOptionalField())
            {
                if (!string.IsNullOrEmpty(fieldValue))
                {
                    return new FormField(parameterInfo.Name, fieldValue, isRequired: false);
                }
                else
                {
                    return new FormField(parameterInfo.Name, parameterInfo.DefaultValue, isRequired: false);
                }
            }
            else
            {
                // Field is required but missing a value
                return new FormField(
                    parameterInfo.Name,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        CommentResources.MissingRequiredValueErrorMessage,
                        parameterInfo.Name));
            }

            bool IsRequiredField() => !string.IsNullOrWhiteSpace(fieldValue) && !parameterInfo.HasDefaultValue;
            bool IsOptionalField() => parameterInfo.HasDefaultValue;
        }

        // TODO: Remove magic string "Email"
        private bool IsEmailValid()
        {
            var email = Fields["email"].Value as string;
            
            if(string.IsNullOrEmpty(email))
            {
                return true;
            }

            return  _validEmail.IsMatch(email);
        }
    }

    public class FormField
    {
        public string Name { get; }
        public object? Value { get; }
        public bool IsRequired { get; }
        public string Error { get; } = string.Empty;
        public bool HasError => !string.IsNullOrEmpty(Error);

        public FormField(string name, object? value, bool isRequired)
        {
            Name = name;
            Value = value;
            IsRequired = isRequired;
        }

        public FormField(string name, string error) :this(name, value: null, isRequired: true)
        {
            Error = error;
        }
    }
}

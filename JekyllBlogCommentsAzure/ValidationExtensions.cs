using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JekyllBlogCommentsAzure
{
    public static class ValidationExtensions
    {
        public static IEnumerable<string> ValidationErrors(this object instance)
        {
            var context = new ValidationContext(instance);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(instance, context, validationResults, validateAllProperties: true);
            foreach (var result in validationResults)
            {
                yield return result.ErrorMessage;
            }
        }
    }
}

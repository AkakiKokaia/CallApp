using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException() : base("Validation failure have occurred.")
        {
        }
        public Dictionary<string, string[]>? ValidationErrors { get; set; } = new();
        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            foreach (var failure in failures.GroupBy(x => x.PropertyName))
            {
                ValidationErrors.Add(failure.Key, failure.Select(x => x.ErrorMessage).ToArray());
            }

        }
        public ValidationException(Dictionary<string, string[]>? failures)
            : this()
        {
            ValidationErrors = failures;

        }

    }
}

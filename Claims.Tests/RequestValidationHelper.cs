using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.Tests
{
    public static class RequestValidationHelper
    {
        public static IList<ValidationResult> Validate(object model)
        {
            ValidationContext context = new(model);
            List<ValidationResult> results = [];

            Validator.TryValidateObject(model, context, results, true);

            return results;
        }
    }
}

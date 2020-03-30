using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Equinor.Procosys.Library.WebApi.Validation
{
    public class ValidationResultModel
    {
        public ValidationResultModel(ModelStateDictionary modelState) =>
            Errors = modelState.Keys
            .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
            .ToList();

        public ValidationResultModel(FluentValidation.ValidationException ve) =>
            Errors = ve.Errors
            .Select(e => new ValidationError(e.PropertyName, e.ErrorMessage))
            .ToList();

        public string Message { get; } = "Validation Failed";
        public List<ValidationError> Errors { get; }

        public override string ToString() => $"{Message}. Errors: {Errors.Select(e => e.ToString())}";
    }
}

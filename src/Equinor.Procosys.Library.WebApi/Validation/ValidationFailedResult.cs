using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Equinor.Procosys.Library.WebApi.Validation
{
    public class ValidationFailedResult : ObjectResult
    {
        protected ValidationFailedResult(ValidationResultModel value)
            : base(value) => StatusCode = StatusCodes.Status422UnprocessableEntity;

        public ValidationFailedResult(ModelStateDictionary modelState)
            : this(new ValidationResultModel(modelState))
        {
        }

        public ValidationFailedResult(FluentValidation.ValidationException ve)
            : this(new ValidationResultModel(ve))
        {
        }
    }
}

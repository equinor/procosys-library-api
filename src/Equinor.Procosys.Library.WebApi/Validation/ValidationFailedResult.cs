using Equinor.Procosys.Library.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Equinor.Procosys.Library.WebApi.Validation
{
    public class ValidationFailedResult : ObjectResult
    {
        public ValidationFailedResult(ModelStateDictionary modelState)
            : base(new ValidationResultModel(modelState))
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity;
        }

        public ValidationFailedResult(DomainException domainException)
            : base(new ValidationResultModel(domainException))
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}

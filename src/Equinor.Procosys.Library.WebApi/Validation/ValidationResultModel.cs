using System.Collections.Generic;
using System.Linq;
using Equinor.Procosys.Library.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Equinor.Procosys.Library.WebApi.Validation
{
    public class ValidationResultModel
    {
        public string Message { get; }

        public List<ValidationError> Errors { get; }

        public ValidationResultModel(DomainException domainException)
        {
            Message = "Error processing the request";
            Errors = new List<ValidationError>
            {
                new ValidationError(null, domainException.Message)
            };
        }

        public ValidationResultModel(ModelStateDictionary modelState)
        {
            Message = "Validation Failed";
            Errors = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                    .ToList();
        }
    }
}

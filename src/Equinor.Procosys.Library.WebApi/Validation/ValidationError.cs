﻿using Newtonsoft.Json;

namespace Equinor.Procosys.Library.WebApi.Validation
{
    public class ValidationError
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; }

        public string Message { get; }

        public ValidationError(string field, string message)
        {
            Field = field != string.Empty ? field : null;
            Message = message;
        }

        public override string ToString() => $"Field: {Field}, Message: {Message}";
    }
}

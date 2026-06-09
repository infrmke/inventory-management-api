using InventoryManagement.Api.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InventoryManagement.Api.Shared.Filters
{
    public class ValidateGuid : ActionFilterAttribute
    {
        private readonly string _parameterName;

        public ValidateGuid(string parameterName = "id")
        {
            _parameterName = parameterName;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // verifica se o parâmetro existe
            if (context.ActionArguments.TryGetValue(_parameterName, out var value))
            {
                var stringValue = value?.ToString();

                if (string.IsNullOrEmpty(stringValue) || !Guid.TryParse(stringValue, out _))
                {
                    throw new BadRequestException($"The ID parameter '{stringValue}' is not a valid UUID/Guid format");
                }
            }
        }
    }
}
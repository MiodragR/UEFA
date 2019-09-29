using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using UEFA.ChampionsLeague.Contracts.DataTransferObjects;

namespace UEFA.ChampionsLeague.Host.Middleware
{
    public class ModelValidationMiddleware : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var eDictionary = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(x => x.Key, x => x.Value.Errors.Select(e => e.ErrorMessage).First());

                var response = new ResponseTemplateViewDto<List<ValidationErrorDto>>
                {
                    IsSuccess = false,
                    ErrorMessage = "Model is not valid",
                    Data = new List<ValidationErrorDto>()
                };

                foreach (var (key, value) in eDictionary)
                {
                    response.Data.Add(new ValidationErrorDto { Key = key, Value = value });
                }

                context.Result = new BadRequestObjectResult(response);
            }
        }
    }
}

﻿using AspNetCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace AspNetCore.WebApi.Filters
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; } = int.MaxValue - 10;

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is null)
                return;

            ObjectResult result;
            switch (context.Exception)
            {
                case BusinessException businessException:
                    result = new ObjectResult(businessException.Message);
                    switch (businessException.Code)
                    {
                        case BusinessExceptions.ProductAlreadyExists:
                            result.StatusCode = (int)HttpStatusCode.Conflict;
                            break;

                        case BusinessExceptions.ProductDoesNotExist:
                            result.StatusCode = (int)HttpStatusCode.NotFound;
                            break;
                    }
                    break;

                default:
                    result = new ObjectResult("Internal server error.") { StatusCode = (int)HttpStatusCode.InternalServerError };
                    break;
            }

            context.Result = result;
            context.ExceptionHandled = true;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }
}

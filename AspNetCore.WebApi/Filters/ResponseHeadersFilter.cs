using AspNetCore.WebApi.DTOs;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AspNetCore.WebApi.Filters
{
    public class ResponseHeadersFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var actionResponsesWithHeaders = context.MethodInfo.GetCustomAttributes<ProducesResponseHeaderAttribute>(true);
            if (!actionResponsesWithHeaders.Any())
                return;

            foreach (var responseCode in operation.Responses.Keys)
            {
                var responseHeaders = actionResponsesWithHeaders.Where(resp => resp.StatusCode.ToString() == responseCode);
                if (!responseHeaders.Any())
                    continue;

                var response = operation.Responses[responseCode];
                if (response.Headers is null)
                    response.Headers = new Dictionary<string, OpenApiHeader>();

                foreach (var responseHeader in responseHeaders)
                {
                    response.Headers[responseHeader.Name] = new OpenApiHeader
                    {
                        Schema = new OpenApiSchema()
                        {
                            Type = responseHeader.Type,
                            Example = GetExampleOrNullFor(responseHeader.Type)
                        },
                        Description = responseHeader.Description
                    };
                }
            }
        }

        private IOpenApiAny GetExampleOrNullFor(string type)
        {
            return type switch
            {
                nameof(PaginationMetadata) => new OpenApiObject
                {
                    ["totalCount"] = new OpenApiInteger(100),
                    ["totalPages"] = new OpenApiInteger(10),
                    ["pageSize"] = new OpenApiInteger(10),
                    ["pageNumber"] = new OpenApiInteger(10),
                    ["hasNextPage"] = new OpenApiBoolean(true),
                    ["hasPreviousPage"] = new OpenApiBoolean(true)
                },
                _ => null,
            };
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class ProducesResponseHeaderAttribute : Attribute
    {
        public ProducesResponseHeaderAttribute(string name, int statusCode)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            StatusCode = statusCode;
        }

        public string Name { get; set; }
        public int StatusCode { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}

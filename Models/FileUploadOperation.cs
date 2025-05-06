using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiHost.Models
{
    public class FileUploadOperation : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasFileUpload = context.MethodInfo
                .GetParameters()
                .Any(p => p.ParameterType == typeof(IFormFile));

            if (!hasFileUpload)
                return;

            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type = "object",
                        Properties =
                        {
                            ["image"] = new OpenApiSchema
                            {
                                Type = "string",
                                Format = "binary"
                            },
                            ["productId"] = new OpenApiSchema
                            {
                                Type = "integer",
                                Format = "int32"
                            }
                        },
                        Required = { "image", "productId" }
                    }
                }
            }
            };
        }
    }

}

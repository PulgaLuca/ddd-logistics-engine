using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LogisticsEngine.Api.Authentication;

/// <summary>Aggiunge lo schema bearer OpenAPI solo alle azioni effettivamente protette.</summary>
public sealed class BearerSecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        bool requiresAuthentication = context.MethodInfo.GetCustomAttributes(inherit: true)
            .OfType<AuthorizeAttribute>().Any();
        if (!requiresAuthentication)
        {
            return;
        }

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = []
        });
    }
}

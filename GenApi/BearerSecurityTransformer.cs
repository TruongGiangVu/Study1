using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace GenApi;

internal sealed class BearerSecurityTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument doc, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        doc.Components ??= new();
        doc.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

        // Define the bearer security scheme
        var securityScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "Enter 'Bearer {token}'",
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };

        doc.Components.SecuritySchemes["Bearer"] = securityScheme;

        // Apply security globally to all endpoints
        var securityRequirement = new OpenApiSecurityRequirement
        {
            [securityScheme] = new List<string>()
        };

        foreach (var path in doc.Paths.Values)
        {
            foreach (var op in path.Operations.Values)
            {
                op.Security ??= new List<OpenApiSecurityRequirement>();
                op.Security.Add(securityRequirement);
            }
        }

        return Task.CompletedTask;
    }
}

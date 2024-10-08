﻿using System.Security.Claims;
using Microsoft.OpenApi;
using NokoWebApiSdk.Annotations;
using NokoWebApiSdk.Extensions.ApiService;
using NokoWebApiSdk.Transformers;

namespace NokoWebApiSdk.Services;

[ApiService]
public class OpenApiService : ApiServiceInitialized
{
    public override void OnInitialized(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthorization((options) =>
        {
            options.AddPolicy("Admin", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypes.Role);
                policy.RequireRole("Admin");
            });
        });

        services.AddEndpointsApiExplorer();
        
        services.AddOpenApi((options) =>
        {
            options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });
    }

    public override void OnConfigure(WebApplication app, IWebHostEnvironment env)
    {
        // TODO: support storing value from scalar options into singleton runtime
        // app.MapOpenApi(pattern: options.OpenApiRoutePattern)
        //     .RequireAuthorization()
        //     .AllowAnonymous();
    }
}
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using NokoWebApi.Controllers;
using NokoWebApi.Models;
using NokoWebApi.Repositories;
using NokoWebApiSdk;
using NokoWebApiSdk.Cores;
using NokoWebApiSdk.Cores.Net;
using NokoWebApiSdk.Extensions.NokoWebApi;
using NokoWebApiSdk.Extensions.ScalarOpenApi.Enums;
using NokoWebApiSdk.Filters;
using NokoWebApiSdk.Generators.Extensions;
using NokoWebApiSdk.Json.Services;
using NokoWebApiSdk.Middlewares;
using NokoWebApiSdk.Repositories;
using NokoWebApiSdk.Schemas;

var noko = NokoWebApplication.Create(args);

noko.Repository((options) =>
{
    // options.UseInMemoryDatabase("main");
    options.UseSqlite("Data Source=Migrations/dev.db");
});

noko.MapOpenApi((options) =>
{
    options.Title = "Scalar API Reference -- {documentName}";
    options.EndpointPathPrefix = "/scalar/{documentName}";
    options.OpenApiRoutePattern = "/openapi/{documentName}.json";
    // options.CdnUrl = "https://cdn.jsdelivr.net/npm/@scalar/api-reference";
    options.CdnUrl = "/js/scalar.api-reference.js";
    options.Theme = ScalarOpenApiTheme.BluePlanet;
    options.Favicon = "/favicon.ico";
});

// Roslyn Source Generator with Reflection 
// must be using external command to generate
// using like dotnet run noko optimize
// noko.Optimize((options) =>
// {
//     options.DirPath = "Optimizes";
//     options.Namespace = "NokoWebApi.Optimizes";
//     options.Cached = true;
// });
// then direct server using type generic
// noko.Run<NokoWebApi.Optimizes.Program>()
// load any environment variables and config files
noko.UseGlobals();

// noko.Listen((app) =>
// {
//     var entryPointAutoGenerated = new NokoWebApi.Optimizes.EntryPointAutoGenerated();
//     entryPointAutoGenerated.OnInitialized(app);
// });

noko.EntryPoint<NokoWebApi.Optimizes.EntryPointAutoGenerated>();

noko.Build();

noko.Application!.Use(HttpExceptionMiddleware.Handler);
noko.Application!.UseMiddleware<CustomExceptionMiddleware>();
noko.Application!.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;
    if (response.StatusCode == (int)HttpStatusCode.NotFound)
    {
        response.ContentType = "application/json";
        var messageBody = new MessageBody<object>
        {
            StatusOk = false,
            StatusCode = response.StatusCode,
            Status = HttpStatusCode.NotFound.ToString(),
            Timestamp = DateTime.UtcNow,
            Message = "Resource not found",
            Data = null
        };

        var options = new JsonSerializerOptions();
        JsonService.JsonSerializerConfigure(options);
        var jsonResponse = JsonSerializer.Serialize(messageBody, options);
        await response.WriteAsync(jsonResponse);
    }
});

noko.Run();

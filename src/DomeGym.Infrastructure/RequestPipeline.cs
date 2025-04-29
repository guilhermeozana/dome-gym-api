using DomeGym.Infrastructure.Middleware;

using Microsoft.AspNetCore.Builder;

namespace DomeGym.Infrastructure;

public static class RequestPipeline
{
    public static IApplicationBuilder AddInfrastructureMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<EventualConsistencyMiddleware>();
        return app;
    }
}
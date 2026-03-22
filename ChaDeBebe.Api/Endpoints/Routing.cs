using Microsoft.EntityFrameworkCore;

public static class RouteManager
{
    public static void MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAuthEndpoints();
        // app.MapProdutoEndpoints();
    }
}
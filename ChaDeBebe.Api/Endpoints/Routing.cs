using Microsoft.EntityFrameworkCore;

public static class RouteManager
{
    public static void MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        app = app.MapGroup("/api");
        app.MapAuthEndpoints();
        app.MapChaDeBebeEndpoints();
        app.MapPresenteEndpoints();
        app.MapReservaEndpoints();
    }
}
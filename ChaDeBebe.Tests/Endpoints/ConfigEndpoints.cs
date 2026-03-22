using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// Configura banco de dados simples para usar nos testes de endpoints
public class IntegrationTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var dbName = Guid.NewGuid().ToString();
        builder.ConfigureServices(services =>
        {
            // 1. Localiza a configuração original do DbContext (Postgres)
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            // 2. Remove essa configuração para o teste não tentar conectar no Docker
            if (descriptor != null) services.Remove(descriptor);

            // 3. Adiciona o banco em memória apenas para o contexto do teste
            // Usamos um nome de banco único (Guid) para que cada teste seja isolado
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(dbName);
            });

            // 4. (Opcional) Cria o banco e garante que as tabelas existam
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
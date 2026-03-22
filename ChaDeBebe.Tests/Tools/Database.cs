using Microsoft.EntityFrameworkCore;

public static class InMemDatabase
{
    public static AppDbContext GetDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Nome único para isolar os testes
            .Options;

        return new AppDbContext(options);
    }
}

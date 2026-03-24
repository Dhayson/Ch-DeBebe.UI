using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<ChaDeBebeEvento> ChasDeBebe { get; set; }
    public DbSet<Presente> Presentes { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<UsuarioChaDeBebe> UsuarioChaDeBebe { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UsuarioChaDeBebe>()
            .HasKey(p => new { p.UsuarioId, p.ChaDeBebeId });
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
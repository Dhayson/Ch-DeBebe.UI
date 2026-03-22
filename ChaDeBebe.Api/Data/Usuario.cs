using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string HashSenha { get; set; } = string.Empty;

    // Construtor vazio para o Entity Framework
    protected Usuario() { }

    public Usuario(string Nome, string Email, string Senha)
    {
        this.Nome = Nome;
        this.Email = Email;
        this.HashSenha = BCrypt.Net.BCrypt.HashPassword(Senha);
    }

    public bool VerificarSenha(string senhaDigitada)
    {
        return BCrypt.Net.BCrypt.Verify(senhaDigitada, this.HashSenha);
    }
}

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(150);

        // Garante que não existam dois emails iguais no banco
        builder.HasIndex(u => u.Email).IsUnique();
    }
}
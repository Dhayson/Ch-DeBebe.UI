using Microsoft.EntityFrameworkCore;

public class UsuarioService
{
    private readonly AppDbContext _db;

    public UsuarioService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Usuario?> BuscarPorId(int id) =>
        await _db.Usuarios.FindAsync(id);

    public async Task<Usuario?> BuscarPorEmail(string email) =>
        await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<Usuario?> Criar(string nome, string email, string senha)
    {
        // O construtor do Usuário já faz o Hash da senha!
        var novoUsuario = new Usuario(nome, email, senha);

        // Assegura que não é criado um usuário com email já existente
        var usuarioExistente = await BuscarPorEmail(email);
        if (usuarioExistente != null)
        {
            return null;
        }

        _db.Usuarios.Add(novoUsuario);
        int nUsuarioCriado = await _db.SaveChangesAsync();

        if (nUsuarioCriado == 0)
        {
            return null;
        }

        return novoUsuario;
    }
}
using System.ComponentModel.DataAnnotations.Schema;

public class ChaDeBebeEvento
{
    // Construtor vazio para o Entity Framework
    protected ChaDeBebeEvento() { }

    public ChaDeBebeEvento(int AdminId, string Nome, DateTime Data)
    {
        this.AdminId = AdminId;
        this.Nome = Nome;
        this.DataEvento = Data;
    }

    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataEvento { get; set; }

    // Relacionamento com o Admin (Usuário)
    public int AdminId { get; set; }

    [ForeignKey("AdminId")]
    public Usuario? Admin { get; set; }

    // Lista de presentes vinculada a este chá específico
    public List<Presente> Presentes { get; set; } = new();
}

public class UsuarioChaDeBebe
{
    // Chave composta no AppDbContext
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public int ChaDeBebeId { get; set; }
    public ChaDeBebeEvento? ChaDeBebeEvento { get; set; }
}
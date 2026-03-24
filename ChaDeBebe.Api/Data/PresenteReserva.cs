using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Presente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public string? Descricao { get; set; }
    public string? LinkSugerido { get; set; }
    public string? PathImage { get; set; }

    // Relacionamento com o Chá
    public int ChaDeBebeEventoId { get; set; }
    [JsonIgnore]
    public ChaDeBebeEvento? ChaDeBebeEvento { get; set; }

    // Relacionamento de Reserva
    [JsonIgnore]
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    public decimal QuantidadeTotal { get; set; } = 1;
    public decimal QuantidadeRestante => QuantidadeTotal - Reservas.Sum(r => r.Quantidade);
    public bool EstaEsgotado => QuantidadeRestante <= 0M;
}

public class Reserva
{
    public int Id { get; set; }
    public decimal Quantidade { get; set; } = 1; // Quantos pacotes a pessoa reservou
    public DateTime DataReserva { get; set; } = DateTime.UtcNow;

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public int ChaDeBebeEventoId { get; set; }
    public ChaDeBebeEvento? ChaDeBebeEvento { get; set; }

    public int PresenteId { get; set; }
    public Presente? Presente { get; set; }


}
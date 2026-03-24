using System.ComponentModel.DataAnnotations;

public record PresenteDTO(
    [Required]
    string Nome,
    string? Descricao,
    string? LinkSugerido,
    string? PathImage,

    [Required]
    int ChaDeBebeEventoId,
    decimal? QuantidadeTotal,
    decimal? Preco
);

public record ReqPresenteDTO(
    [Required]
    int ChaDeBebeEventoId,
    [Required]
    int presenteId
);

public record ReservaDTO(
    decimal Quantidade,
    DateTime DataReserva,
    int UsuarioId,
    int ChaDeBebeEventoId,
    int PresenteId
);
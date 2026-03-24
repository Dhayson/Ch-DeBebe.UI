using System.ComponentModel.DataAnnotations;

public record PresenteDTO(
    [Required]
    string Nome,
    string? Descricao,
    string? LinkSugerido,
    string? PathImage,

    [Required]
    int ChaDeBebeEventoId,
    [Required]
    decimal QuantidadeTotal,
    [Required]
    decimal Preco
);

public record DelPresenteDTO(
    [Required]
    int ChaDeBebeEventoId,
    [Required]
    int presenteId
);
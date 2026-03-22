using System.ComponentModel.DataAnnotations;

public record CriarChaDeBebeDTO(
    [Required]
    string Nome,

    DateTime DataEvento
);
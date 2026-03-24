using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


public class ReservaService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public ReservaService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<(Reserva?, string, int)> AdicionarReservaAsync(ReservaDTO Reserva)
    {
        int usuarioId = Reserva.UsuarioId;
        int chaDeBebeEventoId = Reserva.ChaDeBebeEventoId;
        int presenteId = Reserva.PresenteId;

        var usuarioJaEntrou = await _db.UsuarioChaDeBebe.AnyAsync(
            r => r.UsuarioId == usuarioId && r.ChaDeBebeId == chaDeBebeEventoId);
        if (!usuarioJaEntrou)
        {
            return (null, "Usuário não faz parte desse Chá de Bebê", 400);
        }

        Presente? presente = await _db.Presentes.FirstAsync(
            p => p.Id == presenteId && p.ChaDeBebeEventoId == chaDeBebeEventoId);
        if (presente == null)
        {
            return (null, "Presente não existe nesse Chá de Bebê", 404);
        }

        // Regra de negócio: Não exceder a quantidade máxima
        if (presente.QuantidadeRestante < Reserva.Quantidade)
        {
            return (null, "Reserva excede a quantidade máxima desse presente", 409);
        }

        var reserva = new Reserva
        {
            UsuarioId = usuarioId,
            ChaDeBebeEventoId = chaDeBebeEventoId,
            PresenteId = presenteId,
            Quantidade = Reserva.Quantidade,
            DataReserva = DateTime.UtcNow
        };

        _db.Reservas.Add(reserva);
        _db.Presentes.Update(presente);

        await _db.SaveChangesAsync();

        return (reserva, "Reserva criada com sucesso", 200);
    }
    public async Task<(bool, string, int)> DeletarReservaAsync(int ReservaId)
    {
        (bool, string, int) returnValue;
        var reserva = await _db.Reservas.FirstOrDefaultAsync(r => r.Id == ReservaId);
        if (reserva == null)
        {
            return (false, "Reserva não encontrada", 404);
        }

        var presente = await _db.Presentes.FirstOrDefaultAsync(p => p.Id == reserva.PresenteId);
        if (presente == null)
        {
            returnValue = (true, "Reserva encontrada mas presente não encontrado", 200);
        }
        else
        {
            _db.Presentes.Update(presente);
            returnValue = (true, "Reserva deletada com sucesso", 200);
        }
        _db.Reservas.Remove(reserva);
        await _db.SaveChangesAsync();
        return returnValue;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PYBWeb.Infrastructure.Data;
using PYBWeb.Domain.Interfaces;
using PYBWeb.Domain.Entities;

public class ColaboradorService
{
    private readonly ColaboradoresDbContext _context;
    private readonly ILogService _logService;
    private readonly ICurrentUserService _currentUserService; // ✅ ADICIONAR

    public ColaboradorService(
        ColaboradoresDbContext context, 
        ILogService logService,
        ICurrentUserService currentUserService) // ✅ ADICIONAR
    {
        _context = context;
        _logService = logService;
        _currentUserService = currentUserService; // ✅ ADICIONAR
    }

    public async Task<List<Colaborador>> ObterTodosAsync()
    {
        return await _context.Colaboradores
                             .AsNoTracking()
                             .OrderBy(c => c.Nome)
                             .ToListAsync();
    }

    public async Task<Colaborador?> ObterPorMatriculaAsync(string matricula)
    {
        return await _context.Colaboradores.FindAsync(matricula);
    }

    public async Task AdicionarAsync(Colaborador colaborador, string? usuario = null)
{
    if (colaborador == null) throw new ArgumentNullException(nameof(colaborador));

    // ✅ USA O USUÁRIO PASSADO OU TENTA OBTER DO SERVIÇO
    var usuarioAtual = !string.IsNullOrWhiteSpace(usuario) 
        ? usuario 
        : (_currentUserService.GetCurrentUser() ?? "SISTEMA");

    _context.Colaboradores.Add(colaborador);
    await _context.SaveChangesAsync();

    try
    {
        await _logService.RegistrarAsync(
            acao: "CRIAR",
            tabela: "Colaborador",
            registroId: null,
            registroIdentificador: colaborador.Matricula,
            detalhes: $"Nome={colaborador.Nome};Email={colaborador.Email};Setor={colaborador.Setor}",
            usuario: usuarioAtual
        );
    }
    catch (Exception) { }
}

public async Task AtualizarAsync(Colaborador colaborador, string? usuario = null)
{
    if (colaborador == null) throw new ArgumentNullException(nameof(colaborador));

    // ✅ USA O USUÁRIO PASSADO OU TENTA OBTER DO SERVIÇO
    var usuarioAtual = !string.IsNullOrWhiteSpace(usuario) 
        ? usuario 
        : (_currentUserService.GetCurrentUser() ?? "SISTEMA");

    var existente = await _context.Colaboradores.FindAsync(colaborador.Matricula);

    if (existente == null)
    {
        await AdicionarAsync(colaborador, usuarioAtual);
        return;
    }

    _context.Entry(existente).CurrentValues.SetValues(colaborador);
    await _context.SaveChangesAsync();

    try
    {
        await _logService.RegistrarAsync(
            acao: "EDITAR",
            tabela: "Colaborador",
            registroId: null,
            registroIdentificador: colaborador.Matricula,
            detalhes: $"Nome={colaborador.Nome};Email={colaborador.Email};Setor={colaborador.Setor}",
            usuario: usuarioAtual
        );
    }
    catch (Exception) { }
}

public async Task ExcluirAsync(string matricula, string? usuario = null)
{
    var colaborador = await _context.Colaboradores.FindAsync(matricula);
    if (colaborador != null)
    {
        // ✅ USA O USUÁRIO PASSADO OU TENTA OBTER DO SERVIÇO
        var usuarioAtual = !string.IsNullOrWhiteSpace(usuario) 
            ? usuario 
            : (_currentUserService.GetCurrentUser() ?? "SISTEMA");
        var detalhes = $"Nome={colaborador.Nome};Email={colaborador.Email};Setor={colaborador.Setor}";

        _context.Colaboradores.Remove(colaborador);
        await _context.SaveChangesAsync();

        try
        {
            await _logService.RegistrarAsync(
                acao: "EXCLUIR",
                tabela: "Colaborador",
                registroId: null,
                registroIdentificador: matricula,
                detalhes: detalhes,
                usuario: usuarioAtual
            );
        }
        catch (Exception) { }
    }
}

    public string ObterNomePorMatricula(string matricula)
    {
        var colaborador = _context.Colaboradores.FirstOrDefault(c => c.Matricula == matricula);
        return colaborador?.Nome ?? matricula;
    }
}
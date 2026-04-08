using PYBWeb.Domain.Entities;

namespace PYBWeb.Domain.Interfaces;

/// <summary>
/// Interface para serviço de ambientes CICS
/// </summary>
public interface IAmbienteCicsService
{
    Task<AmbienteCics? > ObterPorNomeAsync(string idChave);
    /// <summary>
    /// Obtém todos os ambientes ativos
    /// </summary>
    Task<IEnumerable<AmbienteCics>> ObterAmbientesAtivosAsync();

    Task<bool> CriarAmbienteAsync(AmbienteCics novoAmbiente);

    /// <summary>
    /// Obtém todos os ambientes (ativos e inativos)
    /// </summary>
    Task<IEnumerable<AmbienteCics>> ObterTodosAmbientesAsync();

    /// <summary>
    /// Obtém ambiente por ID
    /// </summary>
    Task<AmbienteCics?> ObterAmbientePorIdAsync(int id);

    /// <summary>
    /// Obtém ambiente por nome
    /// </summary>
    Task<AmbienteCics?> ObterAmbientePorNomeAsync(string nome);

    /// <summary>
    /// Obtém os ambientes específicos para sistemas PXU/PXS (opção TODOS)
    /// </summary>
    Task<List<AmbienteCics>> ObterAmbientesPxuPxsAsync();

    /// <summary>
    /// Atualiza um ambiente existente
    /// </summary>
    Task<bool> AtualizarAmbienteAsync(AmbienteCics ambiente);

    /// <summary>
    /// Altera o status ativo/inativo de um ambiente
    /// </summary>
    Task<bool> AlterarStatusAmbienteAsync(int id, bool ativo);

    Task<bool> ExcluirAmbienteAsync(int id);

    Task<bool> AlterarStatusPxuAsync(int id, int pxu);
    Task<List<AmbienteCics>> ObterAmbientesPxuAtivosAsync();
}
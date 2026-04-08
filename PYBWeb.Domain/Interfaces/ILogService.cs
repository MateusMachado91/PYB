using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PYBWeb.Domain.Entities;

/// <summary>
/// Interface para serviço de logs de modificações
/// </summary>
public interface ILogService
{
    /// <summary>
    /// Registra uma modificação no sistema
    /// </summary>
    Task RegistrarAsync(string acao, string tabela, int? registroId, string? registroIdentificador,
        string? detalhes = null, string? statusAnterior = null, string? statusNovo = null, string? usuario = null);

    /// <summary>
    /// Obtém logs com filtros opcionais (versão com filtro por usuário)
    /// </summary>
    Task<List<LogModificacao>> ObterLogsAsync(DateTime? dataInicio = null, DateTime? dataFim = null,
        string? usuario = null, string? acao = null, string? tabela = null);

    /// <summary>
    /// Compatibilidade: Obtém logs com filtros opcionais (versão sem filtro por usuário)
    /// </summary>
    Task<List<LogModificacao>> ObterLogsAsync(DateTime? dataInicio = null, DateTime? dataFim = null, string? acao = null, string? tabela = null);

    /// <summary>
    /// Obtém logs de um registro específico
    /// </summary>
    Task<List<LogModificacao>> ObterLogsPorRegistroAsync(string tabela, int registroId);

    /// <summary>
    /// Limpa logs antigos (ex: logs com mais de X dias)
    /// </summary>
    Task LimparLogsAntigosAsync(int dias);

    /// <summary>
    /// Move/arquiva todos os logs atuais para a pasta 'antigos' e esvazia a tabela.
    /// </summary>
    Task LimparLogsAsync();

    /// <summary>
    /// Arquiva (serializa para arquivo em 'antigos') logs com mais de X dias e remove-os do banco.
    /// Retorna a quantidade de registros arquivados.
    /// </summary>
    Task<int> ArquivarLogsAntigosAsync(int dias);

    Task<List<string>> ListarArquivosArquivadosAsync();
    Task<List<LogModificacao>> ObterLogsDoArquivoAsync(string arquivoNome);
}
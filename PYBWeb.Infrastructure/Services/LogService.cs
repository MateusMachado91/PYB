using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PYBWeb.Domain.Entities;
using PYBWeb.Domain.Interfaces;
using PYBWeb.Infrastructure.Data;

namespace PYBWeb.Infrastructure.Services
{
    /// <summary>
    /// Serviço para gerenciamento de logs de modificações
    /// </summary>
    public class LogService : ILogService
    {

        /// <summary>
        /// Lista os arquivos de archive (.db) ordenados por data (mais novos primeiro).
        /// Retorna apenas os nomes dos arquivos (não paths absolutos).
        /// </summary>
        public Task<List<string>> ListarArquivosArquivadosAsync()
        {
            var pasta = ObterArchiveFolderPath();
            if (!Directory.Exists(pasta))
                return Task.FromResult(new List<string>());

            var arquivos = Directory.GetFiles(pasta, "logs*.db", SearchOption.TopDirectoryOnly)
                .OrderByDescending(f => File.GetCreationTimeUtc(f))
                .Select(f => Path.GetFileName(f))
                .ToList();

            return Task.FromResult(arquivos);
        }

        /// <summary>
        /// Lê o arquivo SQLite arquivado e retorna os registros como List<LogModificacao>.
        /// </summary>
        public async Task<List<LogModificacao>> ObterLogsDoArquivoAsync(string arquivoNome)
        {
            if (string.IsNullOrWhiteSpace(arquivoNome))
                throw new ArgumentException("Nome do arquivo inválido.", nameof(arquivoNome));

            // sanitize: evita path traversal
            var nomeSeguro = Path.GetFileName(arquivoNome);
            var arquivoPath = Path.Combine(ObterArchiveFolderPath(), nomeSeguro);

            if (!File.Exists(arquivoPath))
                throw new FileNotFoundException("Arquivo de logs arquivado não encontrado.", arquivoPath);

            var resultados = new List<LogModificacao>();

            var cs = new SqliteConnectionStringBuilder { DataSource = arquivoPath }.ToString();

            try
            {
                using var connection = new SqliteConnection(cs);
                await connection.OpenAsync();

                using var cmd = connection.CreateCommand();
                cmd.CommandText = @"SELECT Id, DataHora, Usuario, Acao, Tabela, RegistroId, RegistroIdentificador, Detalhes, StatusAnterior, StatusNovo
                                    FROM Logs
                                    ORDER BY DataHora DESC;";

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var log = new LogModificacao();

                    // Id
                    if (!reader.IsDBNull(0))
                        log.Id = reader.GetInt32(0);

                    // DataHora stored as TEXT (ISO "o")
                    if (!reader.IsDBNull(1))
                    {
                        var s = reader.GetString(1);
                        if (DateTime.TryParse(s, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt))
                            log.DataHora = dt;
                        else
                            log.DataHora = DateTime.Parse(s); // fallback
                    }

                    log.Usuario = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;
                    log.Acao = !reader.IsDBNull(3) ? reader.GetString(3) : string.Empty;
                    log.Tabela = !reader.IsDBNull(4) ? reader.GetString(4) : string.Empty;
                    log.RegistroId = !reader.IsDBNull(5) ? reader.GetInt32(5) : (int?)null;
                    log.RegistroIdentificador = !reader.IsDBNull(6) ? reader.GetString(6) : null;
                    log.Detalhes = !reader.IsDBNull(7) ? reader.GetString(7) : null;
                    log.StatusAnterior = !reader.IsDBNull(8) ? reader.GetString(8) : null;
                    log.StatusNovo = !reader.IsDBNull(9) ? reader.GetString(9) : null;

                    resultados.Add(log);
                }

                await connection.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler arquivo arquivado '{arquivoPath}': {ex.Message}");
                throw;
            }

            return resultados;
        }

        private readonly LogDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        // Caminho absoluto onde os arquivos de archive serão gravados
        // ALTERE AQUI SE PRECISAR MUDAR NOVAMENTE
      private readonly IConfiguration _configuration;

        public LogService(LogDbContext context, ICurrentUserService currentUserService, IConfiguration configuration)
        {
            _context = context;
            _currentUserService = currentUserService;
            _configuration = configuration;
        }

        private string ObterArchiveFolderPath()
        {
            // Busca da config, ou usa um padrão relativo se não achar
            var pastaAntigos = _configuration.GetValue<string>("PastaLogsAntigos") ?? ".\\DATA_PYB\\LOG\\Antigos";
            return Path.GetFullPath(pastaAntigos);
        }

public async Task RegistrarAsync(
    string acao, 
    string tabela, 
    int? registroId, 
    string? registroIdentificador,
    string? detalhes = null, 
    string? statusAnterior = null, 
    string? statusNovo = null,
    string? usuario = null) // ✅ ADICIONAR
{
    try
    {
        // ✅ SE USUÁRIO FOR PASSADO, USA ELE. SE NÃO, TENTA OBTER DO CONTEXTO
        var usuarioFinal = string.IsNullOrWhiteSpace(usuario) 
            ? _currentUserService.GetCurrentUser() 
            : usuario;

        var log = new LogModificacao
        {
            DataHora = DateTime.Now,
            Usuario = usuarioFinal,
            Acao = acao,
            Tabela = tabela,
            RegistroId = registroId,
            RegistroIdentificador = registroIdentificador,
            Detalhes = detalhes,
            StatusAnterior = statusAnterior,
            StatusNovo = statusNovo
        };

        _context.Logs.Add(log);
        await _context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao registrar log: {ex.Message}");
    }
}
        // Implementação principal com filtro por usuário (opcional)
        public async Task<List<LogModificacao>> ObterLogsAsync(DateTime? dataInicio = null, DateTime? dataFim = null,
            string? usuario = null, string? acao = null, string? tabela = null)
        {
            var query = _context.Logs.AsQueryable();

            if (dataInicio.HasValue)
                query = query.Where(l => l.DataHora >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(l => l.DataHora <= dataFim.Value);

            if (!string.IsNullOrEmpty(usuario))
                query = query.Where(l => l.Usuario.Contains(usuario));

            if (!string.IsNullOrEmpty(acao))
                query = query.Where(l => l.Acao == acao);

            if (!string.IsNullOrEmpty(tabela))
                query = query.Where(l => l.Tabela == tabela);

            return await query
                .OrderByDescending(l => l.DataHora)
                .ToListAsync();
        }

        // Sobrecarga compatível com assinatura alternativa na interface
        public async Task<List<LogModificacao>> ObterLogsAsync(DateTime? dataInicio, DateTime? dataFim, string? acao, string? tabela)
        {
            return await ObterLogsAsync(dataInicio, dataFim, usuario: null, acao: acao, tabela: tabela);
        }

        public async Task<List<LogModificacao>> ObterLogsPorRegistroAsync(string tabela, int registroId)
        {
            return await _context.Logs
                .Where(l => l.Tabela == tabela && l.RegistroId == registroId)
                .OrderByDescending(l => l.DataHora)
                .ToListAsync();
        }

        /// <summary>
        /// Arquiva (copia) logs com DataHora anterior a (Now - dias) para um arquivo SQLite em ArchiveFolderPath
        /// e depois remove esses registros do banco principal. Retorna a quantidade arquivada.
        /// </summary>
        public async Task<int> ArquivarLogsAntigosAsync(int dias)
        {
            var dataLimite = DateTime.Now.AddDays(-dias);

            var logsAntigos = await _context.Logs
                .Where(l => l.DataHora < dataLimite)
                .OrderBy(l => l.DataHora)
                .ToListAsync();

            if (logsAntigos == null || logsAntigos.Count == 0)
                return 0;

            // Usa o caminho absoluto definido por ArchiveFolderPath
            var pastaAntigos = ObterArchiveFolderPath();
            try
            {
                Directory.CreateDirectory(pastaAntigos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Não foi possível criar a pasta de arquivo '{pastaAntigos}': {ex.Message}");
                throw;
            }

            var arquivoNome = $"logs_{DateTime.Now:ddMMyyyy}.db";
            var arquivoPath = Path.Combine(pastaAntigos, arquivoNome);

            Console.WriteLine($"[LogService] Arquivando {logsAntigos.Count} logs em: {arquivoPath}");

            var cs = new SqliteConnectionStringBuilder { DataSource = arquivoPath }.ToString();

            try
            {
                using var connection = new SqliteConnection(cs);
                await connection.OpenAsync();

                using var transaction = connection.BeginTransaction();

                // cria tabela se não existir
                using (var createCmd = connection.CreateCommand())
                {
                    createCmd.Transaction = transaction;
                    createCmd.CommandText =
                        @"CREATE TABLE IF NOT EXISTS Logs (
                            Id INTEGER PRIMARY KEY,
                            DataHora TEXT NOT NULL,
                            Usuario TEXT NOT NULL,
                            Acao TEXT NOT NULL,
                            Tabela TEXT NOT NULL,
                            RegistroId INTEGER,
                            RegistroIdentificador TEXT,
                            Detalhes TEXT,
                            StatusAnterior TEXT,
                            StatusNovo TEXT
                        );";
                    await createCmd.ExecuteNonQueryAsync();
                }

                using (var insertCmd = connection.CreateCommand())
                {
                    insertCmd.Transaction = transaction;
                    insertCmd.CommandText =
                        @"INSERT INTO Logs (Id, DataHora, Usuario, Acao, Tabela, RegistroId, RegistroIdentificador, Detalhes, StatusAnterior, StatusNovo)
                          VALUES ($id, $dataHora, $usuario, $acao, $tabela, $registroId, $registroIdentificador, $detalhes, $statusAnterior, $statusNovo);";

                    // Prepara parâmetros
                    var pId = insertCmd.CreateParameter(); pId.ParameterName = "$id"; insertCmd.Parameters.Add(pId);
                    var pData = insertCmd.CreateParameter(); pData.ParameterName = "$dataHora"; insertCmd.Parameters.Add(pData);
                    var pUsuario = insertCmd.CreateParameter(); pUsuario.ParameterName = "$usuario"; insertCmd.Parameters.Add(pUsuario);
                    var pAcao = insertCmd.CreateParameter(); pAcao.ParameterName = "$acao"; insertCmd.Parameters.Add(pAcao);
                    var pTabela = insertCmd.CreateParameter(); pTabela.ParameterName = "$tabela"; insertCmd.Parameters.Add(pTabela);
                    var pRegistroId = insertCmd.CreateParameter(); pRegistroId.ParameterName = "$registroId"; insertCmd.Parameters.Add(pRegistroId);
                    var pRegistroIdentificador = insertCmd.CreateParameter(); pRegistroIdentificador.ParameterName = "$registroIdentificador"; insertCmd.Parameters.Add(pRegistroIdentificador);
                    var pDetalhes = insertCmd.CreateParameter(); pDetalhes.ParameterName = "$detalhes"; insertCmd.Parameters.Add(pDetalhes);
                    var pStatusAnterior = insertCmd.CreateParameter(); pStatusAnterior.ParameterName = "$statusAnterior"; insertCmd.Parameters.Add(pStatusAnterior);
                    var pStatusNovo = insertCmd.CreateParameter(); pStatusNovo.ParameterName = "$statusNovo"; insertCmd.Parameters.Add(pStatusNovo);

                    foreach (var l in logsAntigos)
                    {
                        pId.Value = l.Id;
                        pData.Value = l.DataHora.ToString("o"); // ISO format
                        pUsuario.Value = l.Usuario;
                        pAcao.Value = l.Acao;
                        pTabela.Value = l.Tabela;
                        pRegistroId.Value = (object?)l.RegistroId ?? DBNull.Value;
                        pRegistroIdentificador.Value = (object?)l.RegistroIdentificador ?? DBNull.Value;
                        pDetalhes.Value = (object?)l.Detalhes ?? DBNull.Value;
                        pStatusAnterior.Value = (object?)l.StatusAnterior ?? DBNull.Value;
                        pStatusNovo.Value = (object?)l.StatusNovo ?? DBNull.Value;

                        await insertCmd.ExecuteNonQueryAsync();
                    }
                }

                await transaction.CommitAsync();
                await connection.CloseAsync();

                // somente após gravação bem-sucedida no arquivo, remove do DB principal
                _context.Logs.RemoveRange(logsAntigos);
                await _context.SaveChangesAsync();

                Console.WriteLine($"[LogService] Arquivamento concluído: {arquivoPath}");
                return logsAntigos.Count;
            }
            catch (Exception ex)
            {
                // se falhar, tenta remover o arquivo parcial
                Console.WriteLine($"Erro ao arquivar logs antigos para SQLite: {ex.Message}");
                try { if (File.Exists(arquivoPath)) File.Delete(arquivoPath); } catch { }
                throw;
            }
        }

        /// <summary>
        /// Delegação para arquivar logs com mais de X dias.
        /// </summary>
        public async Task LimparLogsAntigosAsync(int dias)
        {
            await ArquivarLogsAntigosAsync(dias);
        }

        /// <summary>
        /// Arquiva todos os logs atuais para um arquivo SQLite e limpa a tabela.
        /// </summary>
        public async Task LimparLogsAsync()
        {
            var todosLogs = await _context.Logs
                .OrderBy(l => l.DataHora)
                .ToListAsync();

            if (todosLogs == null || todosLogs.Count == 0)
                return;

            var pastaAntigos = ObterArchiveFolderPath();
            try
            {
                Directory.CreateDirectory(pastaAntigos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Não foi possível criar a pasta de arquivo '{pastaAntigos}': {ex.Message}");
                throw;
            }

            var arquivoNome = $"logs_all_{DateTime.Now:ddMMyyyy}.db";
            var arquivoPath = Path.Combine(pastaAntigos, arquivoNome);

            Console.WriteLine($"[LogService] Arquivando TODOS os logs em: {arquivoPath}");

            var cs = new SqliteConnectionStringBuilder { DataSource = arquivoPath }.ToString();

            try
            {
                using var connection = new SqliteConnection(cs);
                await connection.OpenAsync();

                using var transaction = connection.BeginTransaction();

                using (var createCmd = connection.CreateCommand())
                {
                    createCmd.Transaction = transaction;
                    createCmd.CommandText =
                        @"CREATE TABLE IF NOT EXISTS Logs (
                            Id INTEGER PRIMARY KEY,
                            DataHora TEXT NOT NULL,
                            Usuario TEXT NOT NULL,
                            Acao TEXT NOT NULL,
                            Tabela TEXT NOT NULL,
                            RegistroId INTEGER,
                            RegistroIdentificador TEXT,
                            Detalhes TEXT,
                            StatusAnterior TEXT,
                            StatusNovo TEXT
                        );";
                    await createCmd.ExecuteNonQueryAsync();
                }

                using (var insertCmd = connection.CreateCommand())
                {
                    insertCmd.Transaction = transaction;
                    insertCmd.CommandText =
                        @"INSERT INTO Logs (Id, DataHora, Usuario, Acao, Tabela, RegistroId, RegistroIdentificador, Detalhes, StatusAnterior, StatusNovo)
                          VALUES ($id, $dataHora, $usuario, $acao, $tabela, $registroId, $registroIdentificador, $detalhes, $statusAnterior, $statusNovo);";

                    var pId = insertCmd.CreateParameter(); pId.ParameterName = "$id"; insertCmd.Parameters.Add(pId);
                    var pData = insertCmd.CreateParameter(); pData.ParameterName = "$dataHora"; insertCmd.Parameters.Add(pData);
                    var pUsuario = insertCmd.CreateParameter(); pUsuario.ParameterName = "$usuario"; insertCmd.Parameters.Add(pUsuario);
                    var pAcao = insertCmd.CreateParameter(); pAcao.ParameterName = "$acao"; insertCmd.Parameters.Add(pAcao);
                    var pTabela = insertCmd.CreateParameter(); pTabela.ParameterName = "$tabela"; insertCmd.Parameters.Add(pTabela);
                    var pRegistroId = insertCmd.CreateParameter(); pRegistroId.ParameterName = "$registroId"; insertCmd.Parameters.Add(pRegistroId);
                    var pRegistroIdentificador = insertCmd.CreateParameter(); pRegistroIdentificador.ParameterName = "$registroIdentificador"; insertCmd.Parameters.Add(pRegistroIdentificador);
                    var pDetalhes = insertCmd.CreateParameter(); pDetalhes.ParameterName = "$detalhes"; insertCmd.Parameters.Add(pDetalhes);
                    var pStatusAnterior = insertCmd.CreateParameter(); pStatusAnterior.ParameterName = "$statusAnterior"; insertCmd.Parameters.Add(pStatusAnterior);
                    var pStatusNovo = insertCmd.CreateParameter(); pStatusNovo.ParameterName = "$statusNovo"; insertCmd.Parameters.Add(pStatusNovo);

                    foreach (var l in todosLogs)
                    {
                        pId.Value = l.Id;
                        pData.Value = l.DataHora.ToString("o");
                        pUsuario.Value = l.Usuario;
                        pAcao.Value = l.Acao;
                        pTabela.Value = l.Tabela;
                        pRegistroId.Value = (object?)l.RegistroId ?? DBNull.Value;
                        pRegistroIdentificador.Value = (object?)l.RegistroIdentificador ?? DBNull.Value;
                        pDetalhes.Value = (object?)l.Detalhes ?? DBNull.Value;
                        pStatusAnterior.Value = (object?)l.StatusAnterior ?? DBNull.Value;
                        pStatusNovo.Value = (object?)l.StatusNovo ?? DBNull.Value;

                        await insertCmd.ExecuteNonQueryAsync();
                    }
                }

                await transaction.CommitAsync();
                await connection.CloseAsync();

                // só após gravação com sucesso, remover do DB principal
                _context.Logs.RemoveRange(todosLogs);
                await _context.SaveChangesAsync();

                Console.WriteLine($"[LogService] Arquivamento de todos os logs concluído: {arquivoPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao limpar/arquivar todos os logs para SQLite: {ex.Message}");
                try { if (File.Exists(arquivoPath)) File.Delete(arquivoPath); } catch { }
                throw;
            }
        }
    }
}
using Microsoft.Data.Sqlite;

// Caminho para o banco dados2025.db
var pastaData = Path.Combine(AppContext.BaseDirectory, "..", "DATA_PYB");
var connectionString = Path.Combine(pastaData, "dados2025.db");

try
{
    using var connection = new SqliteConnection(connectionString);
    connection.Open();
    
    Console.WriteLine("=== VERIFICANDO BANCO dados2025.db ===");
    
    // Verifica se a tabela existe
    var checkTableCmd = connection.CreateCommand();
    checkTableCmd.CommandText = @"
        SELECT name FROM sqlite_master 
        WHERE type='table' AND name='SolicitacoesCics';";
    
    var tableName = checkTableCmd.ExecuteScalar() as string;
    
    if (string.IsNullOrEmpty(tableName))
    {
        Console.WriteLine("❌ Tabela SolicitacoesCics NÃO encontrada!");
        
        // Lista todas as tabelas
        var listTablesCmd = connection.CreateCommand();
        listTablesCmd.CommandText = @"
            SELECT name FROM sqlite_master WHERE type='table';";
            
        Console.WriteLine("\n📋 Tabelas existentes:");
        using var reader = listTablesCmd.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine($"  - {reader.GetString(0)}");
        }
    }
    else
    {
        Console.WriteLine("✅ Tabela SolicitacoesCics encontrada!");
        
        // Conta registros
        var countCmd = connection.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM SolicitacoesCics;";
        var count = (long)countCmd.ExecuteScalar();
        
        Console.WriteLine($"📊 Total de registros: {count}");
        
        if (count > 0)
        {
            // Mostra alguns registros
            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = @"
                SELECT NumeroSolicitacao, Usuario, Css, TipoTabela, Status, DataSolicitacao 
                FROM SolicitacoesCics 
                ORDER BY DataSolicitacao DESC 
                LIMIT 5;";
                
            Console.WriteLine("\n📝 Últimas 5 solicitações:");
            using var dataReader = selectCmd.ExecuteReader();
            while (dataReader.Read())
            {
                Console.WriteLine($"  - {dataReader.GetString(0)} | {dataReader.GetString(1)} | {dataReader.GetString(2)} | {dataReader.GetString(3)} | {dataReader.GetString(4)} | {dataReader.GetDateTime(5):dd/MM/yyyy HH:mm}");
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Erro ao acessar banco: {ex.Message}");
    Console.WriteLine($"StackTrace: {ex.StackTrace}");
}

Console.WriteLine("\n✅ Verificação concluída!");
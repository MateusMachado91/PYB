# Sistema de Logs - Guia de Uso

## 📋 Como Registrar Logs

### Injetar o serviço na sua página/componente:
```csharp
@inject ILogService LogService
```

### Exemplos de uso:

#### 1. Mudança de Status de Solicitação
```csharp
await LogService.RegistrarAsync(
    acao: "ALTERAR_STATUS",
    tabela: "SolicitacaoCics2025",
    registroId: solicitacao.Id,
    registroIdentificador: solicitacao.NumeroSolicitacao,
    detalhes: $"Alterado status de {statusAnterior} para {statusNovo}",
    statusAnterior: statusAnterior,
    statusNovo: statusNovo
);
```

#### 2. Gerar JCL
```csharp
await LogService.RegistrarAsync(
    acao: "GERAR_JCL",
    tabela: "SolicitacaoCics2025",
    registroId: solicitacao.Id,
    registroIdentificador: solicitacao.NumeroSolicitacao,
    detalhes: $"JCL gerado para tabela {solicitacao.TipoTabela} - Arquivo: {nomeArquivo}"
);
```

#### 3. Editar Ambiente CICS
```csharp
await LogService.RegistrarAsync(
    acao: "EDITAR",
    tabela: "AmbienteCics",
    registroId: ambiente.Id,
    registroIdentificador: ambiente.Sid,
    detalhes: $"Ambiente {ambiente.Sid} atualizado. Applid: {ambiente.Applid}"
);
```

#### 4. Criar Nova Solicitação
```csharp
await LogService.RegistrarAsync(
    acao: "CRIAR",
    tabela: "SolicitacaoCics2025",
    registroId: novaSolicitacao.Id,
    registroIdentificador: novaSolicitacao.NumeroSolicitacao,
    detalhes: $"Nova solicitação criada para tabela {novaSolicitacao.TipoTabela}"
);
```

#### 5. Excluir Registro
```csharp
await LogService.RegistrarAsync(
    acao: "EXCLUIR",
    tabela: "AmbienteTodos",
    registroId: ambiente.Id,
    registroIdentificador: ambiente.SysidRemoto,
    detalhes: $"Ambiente remoto {ambiente.SysidRemoto} excluído"
);
```

## 🎯 Tipos de Ações Recomendadas

- **CRIAR** - Criar novo registro
- **EDITAR** - Atualizar registro existente
- **EXCLUIR** - Remover registro
- **ALTERAR_STATUS** - Mudança de status (Pendente → Aprovada, etc)
- **GERAR_JCL** - Geração de arquivo JCL
- **APROVAR** - Aprovação de solicitação
- **REJEITAR** - Rejeição de solicitação
- **ATIVAR** - Ativar registro
- **DESATIVAR** - Desativar registro

## 📊 Consultar Logs

```csharp
// Todos os logs de hoje
var logsHoje = await LogService.ObterLogsAsync(
    dataInicio: DateTime.Today
);

// Logs de um usuário específico
var logsUsuario = await LogService.ObterLogsAsync(
    usuario: "CORP\\E38235"
);

// Logs de uma solicitação específica
var logsSolicitacao = await LogService.ObterLogsPorRegistroAsync(
    tabela: "SolicitacaoCics2025",
    registroId: 123
);

// Logs de alterações de status
var logsStatus = await LogService.ObterLogsAsync(
    acao: "ALTERAR_STATUS"
);
```

## 🧹 Limpeza de Logs Antigos

```csharp
// Limpar logs com mais de 12 meses
await LogService.LimparLogsAntigosAsync(meses: 12);
```

## 📍 Onde Adicionar os Logs

### Home.razor - AlterarStatusSolicitacao
```csharp
// Após salvar a mudança de status
await LogService.RegistrarAsync("ALTERAR_STATUS", "SolicitacaoCics2025", ...);
```

### Implementacoes/Nova.razor - GerarJCL
```csharp
// Após gerar o JCL
await LogService.RegistrarAsync("GERAR_JCL", "SolicitacaoCics2025", ...);
```

### Ambientes/Index.razor - SalvarEdicao
```csharp
// Após editar ambiente
await LogService.RegistrarAsync("EDITAR", "AmbienteCics", ...);
```

## 💾 Localização do Banco

O arquivo de logs está em: `X:\DATA_PYB\LOG\logs.db`


Para testar a página de acesso negado:

Acesse manualmente: http://localhost:porta/acesso-negado
Em produção, para habilitar a segurança:

No Program.cs:
Descomentar AddAuthentication e AddNegotiate()
Descomentar app.UseAuthentication() e app.UseAuthorization()
Em todas as páginas (Home, Histórico, Implementações, Ambientes, Logs):
Descomentar @attribute [Authorize(Policy = "RequireAdmin")]
Assim:

✅ Agora funciona sem autenticação para testes
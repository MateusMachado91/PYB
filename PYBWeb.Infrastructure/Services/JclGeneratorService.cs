using PYBWeb.Domain.Entities;
using PYBWeb.Domain.Interfaces;
using System.Text;

namespace PYBWeb.Infrastructure.Services
{
    public class JclGeneratorService : IJclGeneratorService
    {
        private readonly IAmbienteCicsService _ambienteCicsService;

        public JclGeneratorService(IAmbienteCicsService ambienteCicsService)
        {
            _ambienteCicsService = ambienteCicsService;
        }

        /// <summary>
        /// Gera um único JCL consolidado para uma ou múltiplas solicitações (DEFINE, ALTER, DELETE)
        /// </summary>
            #region GERA GRUPO JCL
            #endregion
        public string GerarJclGrupo(List<SolicitacaoCics2025> grupo, AmbienteCics ambiente)
        {
            if (grupo == null || !grupo.Any() || ambiente == null)
                return string. Empty;

            var maquina = ambiente.Maquina ?? "??? ";
            var steplibCsd = ambiente.SteplibCsd ?? "";
            var dsnameDfhcsd = ambiente.DsnameDfhcsd ?? "";
            var sufixo = ambiente.Sufixo ??  "";
            
            var sb = new StringBuilder();
            
            #region CABEÇALHO JCL
            #endregion
            sb.AppendLine($"//PYBCSD2{sufixo} JOB  PYB2-00952400-0019,'UAC-SAP',MSGCLASS=R,");
            sb.AppendLine("//         CLASS=S,TIME=3,REGION=4M");
            sb.AppendLine($"/*XEQ JES{maquina}");
            sb.AppendLine($"/*ROUTE PRINT JESD");
            sb.AppendLine("//*        UPCASE=NAO");
            sb.AppendLine("//         EXEC PGM=DFHCSDUP");
            sb.AppendLine($"//STEPLIB  DD   DSN={steplibCsd},DISP=SHR");
            sb.AppendLine($"//DFHCSD   DD   DSN={dsnameDfhcsd},DISP=SHR");
            sb.AppendLine("//SYSPRINT DD   SYSOUT=*");
            sb.AppendLine("//SYSIN    DD   *");
            sb.AppendLine();
            
            // ✅ PROCESSAR CADA SOLICITAÇÃO
            foreach (var sol in grupo. OrderBy(s => s.TipoTabela).ThenBy(s => s.Operacao))
            {
                AppendComandosJcl(sb, sol, ambiente);
                
                sb.AppendLine();
            }
            
            return sb.ToString();
        }

        /// <summary>
        /// Router:  direciona para o método correto conforme tipo e operação
        /// </summary>
            #region ROTA DO JCL
            #endregion
        private void AppendComandosJcl(StringBuilder sb, SolicitacaoCics2025 sol, AmbienteCics ambiente)
        {
            var operacao = sol.Operacao?.ToUpperInvariant() ?? "DEFINE";
            var tipoTabela = sol.TipoTabela?.ToUpperInvariant() ?? "";
            var type = sol.Type?.ToUpperInvariant() ?? "";
            var css = sol.Css ??  "";
            var sufixo = ambiente.Sufixo ?? "";
            var maquina = ambiente.Maquina ?? "";

            if (tipoTabela == "ARQUIVO")
            {
                if (operacao == "DELETE")
                    AppendDeleteArquivo(sb, sol, css, sufixo, maquina);
                else if (type == "LOCAL")
                    AppendDefineArquivoLocal(sb, sol, css, sufixo, operacao);
                else if (type == "REMOTO")
                    AppendDefineArquivoRemoto(sb, sol, css, sufixo, operacao);
                else if (type == "PADRÃO BNO")
                    AppendDefineArquivoBno(sb, sol, ambiente, operacao);
            }
            else if (tipoTabela == "TRANSACAO")
            {
                if (operacao == "DELETE")
                    AppendDeleteTransacao(sb, sol, css, sufixo, maquina);
                else
                    AppendDefineTransacao(sb, sol, css, operacao);
            }
            else if (tipoTabela == "PROGRAMA")
            {
                if (operacao == "DELETE")
                    AppendDeletePrograma(sb, sol, css, maquina, sufixo);
                else
                    AppendDefinePrograma(sb, sol, css, maquina, sufixo, operacao);
            }
            else if (tipoTabela == "MAPA")
            {
                if (operacao == "DELETE")
                    AppendDeleteMapa(sb, sol, css, maquina, sufixo);
                else
                    AppendDefineMapa(sb, sol, css, maquina, sufixo, operacao);
            }
            else if (tipoTabela == "TABELA")
            {
                if (operacao == "DELETE")
                    AppendDeleteTabela(sb, sol, css, maquina, sufixo);
                else
                    AppendDefineTabela(sb, sol, css, maquina, sufixo, operacao);
            }
        }

            #region ARQUIVO
            #endregion
        private void AppendDeleteArquivo(StringBuilder sb, SolicitacaoCics2025 sol, string css, string sufixo, string maquina)
        {
            var names = (sol.NameArq ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var n in names)
            {
                sb.AppendLine($"DELETE  FILE({n})");
                sb. AppendLine($"        GROUP(FCT{css}A)");
            }
        }
            #region ARQUIVO LOCAL
            #endregion
        private void AppendDefineArquivoLocal(StringBuilder sb, SolicitacaoCics2025 sol, string css, string sufixo, string operacao)
        {
            var name = (sol.NameArq ?? "").ToUpperInvariant();
            var status = sol.EstInit ?? "ENABLE";
            var recfm = sol.FormReg ?? "FIXED";
            var strg = sol.NumStrng ?? "03";
            var dsname = sol.DsnameArq ?? "";
            
            var servicos = ! string.IsNullOrWhiteSpace(sol.Service)
                ? sol.Service.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                : Array.Empty<string>();

            sb.AppendLine($"{operacao.PadRight(7)} FILE({name})");
            sb.AppendLine($"        DSNAME({dsname})");
            sb.AppendLine($"        STATUS({status})");
            sb.AppendLine($"        RECORDFORMAT({recfm[0]})");

            foreach (var svc in servicos)
                sb.AppendLine($"        {svc. ToUpper()}(YES)");

            sb.AppendLine($"        STRINGS({strg})");
            sb.AppendLine("        LSRPOOLID(NONE)");
            sb.AppendLine($"        GROUP(FCT{css}A{sufixo})");
        }

            #region ARQUIVO REMOTO
            #endregion
        private void AppendDefineArquivoRemoto(StringBuilder sb, SolicitacaoCics2025 sol, string css, string sufixo, string operacao)
        {
            var name = (sol.NameArq ??  "").ToUpperInvariant();
            
            string codigoISC = "??? ";
            
            if (!string.IsNullOrWhiteSpace(sol.DsnameArq))
            {
                var ambienteRemoto = _ambienteCicsService
                    .ObterPorNomeAsync(sol.DsnameArq)
                    .GetAwaiter()
                    .GetResult();
                
                codigoISC = ambienteRemoto?.Isc ??  sol.DsnameArq;
            }

            sb.AppendLine($"{operacao.PadRight(7)} FILE({name})");
            sb.AppendLine($"        REMOTESYSTEM({codigoISC})");
            sb.AppendLine("        LSRPOOLID(NONE)");
            sb.AppendLine($"        GROUP(FCT{css}A{sufixo})");
        }

            #region ARQUIVO BNO
            #endregion
        private void AppendDefineArquivoBno(StringBuilder sb, SolicitacaoCics2025 sol, AmbienteCics ambiente, string operacao)
        {
            var agencia = sol.FileName?.PadLeft(4, '0') ?? "0000";
            var tipos = (sol.FormReg2 ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var status = sol.EstInit ?? "ENABLE";
            var recfm = sol.FormReg ?? "FIXED";
            var strg = sol.NumStrng ?? "03";
            var maquina = ambiente.Maquina ?? "???";

            foreach (var tipo in tipos)
            {
                var nome = $"BNO{tipo}{agencia}";
                var dsname = tipo switch
                {
                    "I" => $"BPDBNO{maquina}.DII001.G00000.AGE{agencia}",
                    "O" => $"BPDBNO{maquina}.DOI001.G00000.AGE{agencia}",
                    "P" => $"BPDBNO{maquina}.DPI001.G00000.AGE{agencia}",
                    "S" => $"BPDBNO{maquina}.DSI001.G00000.AGE{agencia}",
                    "X" => $"BPDBNO{maquina}.DHI001.G00000.AGE{agencia}",
                    _ => "?"
                };

                string[] servicos = tipo switch
                {
                    "I" => new[] { "BROWSE", "READ", "UPDATE" },
                    "O" => new[] { "ADD", "BROWSE", "DELETE", "READ", "UPDATE" },
                    "S" => new[] { "ADD", "BROWSE", "READ", "UPDATE" },
                    "P" => new[] { "ADD", "BROWSE", "READ", "UPDATE" },
                    "X" => new[] { "BROWSE", "READ" },
                    _ => Array.Empty<string>()
                };

                sb.AppendLine($"{operacao.PadRight(7)} FILE({nome})");
                sb. AppendLine($"        DSNAME({dsname})");
                sb.AppendLine($"        STATUS({status})");
                sb.AppendLine($"        RECORDFORMAT({recfm[0]})");

                foreach (var svc in servicos)
                    sb.AppendLine($"        {svc}(YES)");

                sb. AppendLine($"        STRINGS({strg})");
                sb.AppendLine("        LSRPOOLID(NONE)");
                sb.AppendLine("        GROUP(FCTBNO)");
                sb.AppendLine(maquina == "A" ? "        REMOTESystem(CICF)" : "        REMOTESystem(DCIF)");
            }
        }

            #region TRANSAÇÃO
            #endregion
        
        private void AppendDeleteTransacao(StringBuilder sb, SolicitacaoCics2025 sol, string css, string sufixo, string maquina)
        {
            var names = (sol.NameTrans ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var n in names)
            {
                sb.AppendLine($"DELETE  TRANSACTION({n})");
                sb. AppendLine($"        GROUP({css}{sufixo})");
            }
        }

        private void AppendDefineTransacao(StringBuilder sb, SolicitacaoCics2025 sol, string css, string operacao)
        {
            var nameTrans = (sol.NameTrans ?? "TRAN").ToUpperInvariant();
            var program = sol.ActiveSoft ??  "PROGRAM";
            var twasize = sol.TwaSize ?? "0";
            var status = sol.EstInit ?? "ENABLE";
            var prev = sol. Prev ?? "0";
            var data = sol.DataAllocation ?? "ANY";

            sb.AppendLine($"{operacao.PadRight(7)} TRANSACTION({nameTrans})");
            sb.AppendLine($"        PROGRAM({program})");
            sb.AppendLine($"        TWASIZE({twasize})");
            sb.AppendLine($"        STATUS({status})");
            sb.AppendLine($"        TASKDATALOC({data})");
            sb.AppendLine($"        DESCRIPTION(Previsao:  {prev} execucoes dia)");
            sb.AppendLine("        SPURGE(YES)");
            sb.AppendLine("        TPURGE(YES)");
            sb.AppendLine("        ISOLATE(NO)");
            sb.AppendLine($"        GROUP({css})");
        }

            #region PROGRAMA
            #endregion
        
        private void AppendDeletePrograma(StringBuilder sb, SolicitacaoCics2025 sol, string css, string maquina, string sufixo)
        {
            var names = (sol.NameSoft ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var n in names)
            {
                sb.AppendLine($"DELETE  PROGRAM({n})");
                sb. AppendLine("        CONCURRENCY(THREADSAFE)");
                sb.AppendLine($"        GROUP({css}{maquina}{sufixo})");
            }
        }

        private void AppendDefinePrograma(StringBuilder sb, SolicitacaoCics2025 sol, string css, string maquina, string sufixo, string operacao)
        {
            var name = (sol.NameSoft ?? "").ToUpperInvariant();
            var status = sol. EstInit ?? "ENABLE";
            var data = sol.DataAllocation ?? "ANY";
            var dsname = sol.LinkName ?? "";
            var cod = sol.Language ?? "COBOL";

            sb.AppendLine($"{operacao.PadRight(7)} PROGRAM({name})");
            sb.AppendLine("        CONCURRENCY(THREADSAFE)");
            sb.AppendLine($"        LANGUAGE({cod})");
            sb.AppendLine($"        STATUS({status})");
            sb.AppendLine($"        DATALOC({data})");
            sb.AppendLine("        RESIDENT(NO)");
            sb.AppendLine($"        DESCRIPTION({dsname})");
            sb.AppendLine($"        GROUP({css}{maquina}{sufixo})");
        }

            #region MAPA
            #endregion
        
        private void AppendDeleteMapa(StringBuilder sb, SolicitacaoCics2025 sol, string css, string maquina, string sufixo)
        {
            var names = (sol.NameSoft ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var n in names)
            {
                sb.AppendLine($"DELETE  MAPSET({n})");
                sb.AppendLine($"        GROUP({css}{maquina}{sufixo})");
            }
        }

        private void AppendDefineMapa(StringBuilder sb, SolicitacaoCics2025 sol, string css, string maquina, string sufixo, string operacao)
        {
            var name = (sol.NameSoft ?? "").ToUpperInvariant();
            var status = sol.EstInit ?? "ENABLE";
            var link = sol.LinkName ?? "";

            sb.AppendLine($"{operacao.PadRight(7)} MAPSET({name})");
            sb.AppendLine($"        STATUS({status})");
            sb.AppendLine("        RESIDENT(NO)");
            sb.AppendLine($"        DESCRIPTION({link})");
            sb.AppendLine($"        GROUP({css}{maquina}{sufixo})");
        }

            #region TABELA
            #endregion
        
        private void AppendDeleteTabela(StringBuilder sb, SolicitacaoCics2025 sol, string css, string maquina, string sufixo)
        {
            var names = (sol.NameSoft ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var n in names)
            {
                sb.AppendLine($"DELETE  PROGRAM({n})");
                sb. AppendLine("        CONCURRENCY(THREADSAFE)");
                sb.AppendLine($"        GROUP({css})");
            }
        }

        private void AppendDefineTabela(StringBuilder sb, SolicitacaoCics2025 sol, string css, string maquina, string sufixo, string operacao)
        {
            var name = (sol. NameSoft ?? "").ToUpperInvariant();
            var status = sol.EstInit ?? "ENABLE";
            var link = sol.LinkName ?? "";
            var cod = sol.Language ?? "";

            sb.AppendLine($"{operacao.PadRight(7)} PROGRAM({name})");
            sb.AppendLine("        CONCURRENCY(THREADSAFE)");
            sb.AppendLine($"        LANGUAGE({cod})");            
            sb.AppendLine($"        STATUS({status})");
            sb.AppendLine($"        DATALOC(ANY)");            
            sb.AppendLine("        RESIDENT(NO)");
            sb.AppendLine($"        DESCRIPTION({link})");
            sb.AppendLine($"        GROUP({css})");
        }
    }
}

            #region FIM JCL
            #endregion
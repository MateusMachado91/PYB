namespace PYBWeb.Infrastructure.Services
{
    public static class RegrasTabelas
    {
        // Regras para nome da fila
        public static bool ValidarNomeFila(string? nomeFila, string? css)
        {
            if (string.IsNullOrWhiteSpace(nomeFila))
                return false;

            // Regra 1: Deve ter exatamente 4 caracteres
            if (nomeFila.Length != 4)
                return false;

            // Regra 2: Não pode começar com 'C'
            if (nomeFila.StartsWith("C", StringComparison.OrdinalIgnoreCase))
                return false;

            // Regra 3: Se sistema for PCG
            if (css?.ToUpperInvariant() == "PCG")
            {
                if (!nomeFila.StartsWith("P", StringComparison.OrdinalIgnoreCase))
                    return false;

                // Últimos dois caracteres devem coincidir com os dois últimos do CSS
                if (nomeFila.Substring(2, 2).ToUpperInvariant() != css.Substring(css.Length - 2).ToUpperInvariant())
                    return false;
            }

            return true;
        }

        // Regras para DDNAME do tipo EXTRA
        public static bool ValidarDdnameExtra(string? ddname, string? css)
        {
            if (string.IsNullOrWhiteSpace(ddname))
                return false;

            ddname = ddname.ToUpperInvariant();

            // Opção 1: 'A,INTRDR'
            if (ddname.Equals("A,INTRDR", StringComparison.OrdinalIgnoreCase))
                return true;

            // Opção 2: Nome que começe com 'BPDcss', contém '.S' e '.G00000'
            if (!string.IsNullOrWhiteSpace(css))
            {
                var cssUpper = css.ToUpperInvariant();
                if (ddname.StartsWith($"BPD{cssUpper}") &&
                    ddname.Contains(".S") &&
                    ddname.Contains(".G00000"))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Valida o tamanho do registro.
        /// Deve ser maior que 0 e menor que 32768.
        /// </summary>
        public static bool ValidarTamanhoRegistro(string? regSize)
        {
            if (string.IsNullOrWhiteSpace(regSize))
                return false;

            if (!int.TryParse(regSize, out int tamanho))
                return false;

            return tamanho > 0 && tamanho < 32768;
        }

        /// <summary>
        /// Valida o tamanho do bloco.
        /// Se BLOCK: 1 a 32767 | Se UNBLOCK: 0
        /// </summary>
        public static bool ValidarTamanhoBloco(string? blockSize, string? formReg2)
        {
            if (string.IsNullOrWhiteSpace(blockSize))
                return false;

            if (!int.TryParse(blockSize, out int tamanho))
                return false;

            if (formReg2?.ToUpperInvariant() == "BLOCK")
                return tamanho >= 1 && tamanho <= 32767;

            if (formReg2?.ToUpperInvariant() == "UNBLOCK")
                return tamanho == 0;

            return true;
        }

        // Regras para INTRA

        /// <summary>
        /// Valida a transação (campo de texto não vazio).
        /// </summary>
        public static bool ValidarTransacao(string? transacao)
        {
            return !string.IsNullOrWhiteSpace(transacao);
        }

        /// <summary>
        /// Valida o TrigLevel.
        /// Deve ser maior que zero.
        /// </summary>

        

        public static bool ValidarTrigLevel(int? trigLevel)
        {
            return trigLevel.HasValue && trigLevel.Value > 0;
        }


        /// <summary>
        /// Valida o destino.
        /// Deve ser "TERMINAL" ou "FILE".
        /// </summary>
        public static bool ValidarDestino(string? destino)
        {
            if (string.IsNullOrWhiteSpace(destino))
                return false;

            var valor = destino.ToUpperInvariant();
            return valor == "TERMINAL" || valor == "FILE";
        }

        /// <summary>
        /// Valida o terminal.
        /// Obrigatório se destino for TERMINAL.
        /// Deve ter exatamente 4 caracteres.
        /// </summary>
        public static bool ValidarTerminal(string? terminal, string? destino)
        {
            if (destino?.ToUpperInvariant() != "TERMINAL")
                return true; // Não precisa validar terminal se destino não for TERMINAL

            if (string.IsNullOrWhiteSpace(terminal))
                return false;

            return terminal.Length == 4;
        }
        
        public static bool ValidarFilaIntra(string? transacao, int? trigLevel, string? destino, string? terminal)
        {
            return ValidarTransacao(transacao)
                && ValidarTrigLevel(trigLevel)
                && ValidarDestino(destino)
                && ValidarTerminal(terminal, destino);
        }

        public static bool ValidarNomeTransacao(string? nameTrans, string? css)
        {
            if (string.IsNullOrWhiteSpace(nameTrans) || string.IsNullOrWhiteSpace(css) || css.Length < 2)
                return false;

            var ultimosDois = css.Substring(css.Length - 2);
            return nameTrans.StartsWith(ultimosDois, StringComparison.OrdinalIgnoreCase)
                || nameTrans.EndsWith(ultimosDois, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ValidarProgramaParaAtivar(string? program, string? css)
        {
            if (string.IsNullOrWhiteSpace(program) || string.IsNullOrWhiteSpace(css))
                return false;

            program = program.ToUpperInvariant();
            css = css.ToUpperInvariant();

            // Verifica se começa com CSS + "P" ou "PWXP"
            return program.StartsWith(css + "P") || program.StartsWith("PWXP");
        }

        public static bool ValidarTwaSize(string? twaSize)
        {
            if (string.IsNullOrWhiteSpace(twaSize))
                return false;

            if (!int.TryParse(twaSize, out int valor))
                return false;

            return valor < 32768;
        }

        public static bool ValidarNomeArquivoFct(string? nameArq, string? css)
        {
            if (string.IsNullOrWhiteSpace(nameArq) || string.IsNullOrWhiteSpace(css))
                return false;
            var nomeArquivo = nameArq.ToUpperInvariant();
            var cssUpper = css.ToUpperInvariant();
            return nomeArquivo.StartsWith(cssUpper);
        }

        public static bool ValidarDsnameFct(string? dsnameArq, string? css)
        {
            if (string.IsNullOrWhiteSpace(dsnameArq) || string.IsNullOrWhiteSpace(css))
                return false;
            var dsname = dsnameArq.ToUpperInvariant();
            var cssUpper = css.ToUpperInvariant();
            return
                dsname.StartsWith("BPD" + cssUpper) &&
                dsname.Contains(".D") &&
                dsname.Contains(".G00000") &&
                !dsname.Contains("_");
        }

        // ========== VALIDAÇÕES FCT LOCAL ==========
        
        public static (bool isValid, string errorMessage) ValidarServicesFctLocal(bool isRead, bool isUpdate, bool isDelete, bool isBrowse, bool isAdd)
        {
            var services = new[] { isRead, isUpdate, isDelete, isBrowse, isAdd };
            if (!services.Any(s => s))
            {
                return (false, "Pelo menos um dos 5 serviços deve ser selecionado");
            }
            return (true, string.Empty);
        }

        public static (bool isValid, string errorMessage) ValidarNumStrngFctLocal(string numStrng, bool isBrowse)
        {
            if (string.IsNullOrWhiteSpace(numStrng))
            {
                return (false, "Numero de String é obrigatório");
            }

            if (!int.TryParse(numStrng, out int valor))
            {
                return (false, "Numero de String deve ser um número válido");
            }

            if (valor < 1 || valor > 10)
            {
                return (false, "Numero de Stringdeve estar entre 1 e 10");
            }

            if (isBrowse && valor < 3)
            {
                return (false, "Numero de String deve ser ≥ 3 quando BROWSE está selecionado");
            }

            return (true, string.Empty);
        }

        // ========== VALIDAÇÕES PPT ==========

        public static (bool isValid, string errorMessage) ValidarNomeProgramaPpt(string nomePrograma, string css)
        {
            if (string.IsNullOrWhiteSpace(nomePrograma))
            {
                return (false, "Nome do Programa é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(css))
            {
                return (false, "CSS é obrigatório para validar o Nome do Programa");
            }

            var programa = nomePrograma.ToUpperInvariant();
            var cssUpper = css.ToUpperInvariant();

            if (!programa.StartsWith(cssUpper))
            {
                return (false, $"Nome do Programa deve começar com '{cssUpper}'");
            }

            return (true, string.Empty);
        }

        public static (bool isValid, string errorMessage) ValidarNomeLinkPpt(string nomeLink, string css)
        {
            if (string.IsNullOrWhiteSpace(nomeLink))
            {
                return (false, "Nome da Link é obrigatório");
            }

            if (string.IsNullOrWhiteSpace(css))
            {
                return (false, "CSS é obrigatório para validar o Nome da Link");
            }

            var link = nomeLink.ToUpperInvariant();
            var cssUpper = css.ToUpperInvariant();

            // Verifica se começa com "BPDcss" ou é uma das exceções
            var comecaComBpdCss = link.StartsWith("BPD" + cssUpper);
            var isExcecao = link == "BPDPXS" || link == "BPDPSWB.BLINK1.G00000.ODEII";

            if (!comecaComBpdCss && !isExcecao)
            {
                return (false, $"Nome da Link deve começar com 'BPD{cssUpper}' ou ser 'BPDPXS' ou 'BPDPSWB.BLINK1.G00000.ODEII'");
            }

            // Verifica se contém .BLKCI ou .BLINK
            var contemBlkci = link.Contains(".BLKCI");
            var contemBlink = link.Contains(".BLINK");

            if (!contemBlkci && !contemBlink)
            {
                return (false, "Nome da Link deve conter '.BLKCI' ou '.BLINK'");
            }

            // Verifica se contém .G00000
            if (!link.Contains(".G00000"))
            {
                return (false, "Nome da Link deve conter '.G00000'");
            }

            return (true, string.Empty);
        }

        public static (bool isValid, string errorMessage) ValidarAutoAlteravelPpt(bool isAutoAlteravel, string justificativaAutoAlteravel)
        {
            if (isAutoAlteravel && string.IsNullOrWhiteSpace(justificativaAutoAlteravel))
            {
                return (false, "Justificativa é obrigatória quando Auto-Alterável está marcado");
            }

            return (true, string.Empty);
        }

        public static (bool isValid, string errorMessage) ValidarAlocacaoDadosPpt(string alocacaoDados, string justificativaBelow)
        {
            if (string.IsNullOrWhiteSpace(alocacaoDados))
            {
                return (false, "Alocação dos Dados é obrigatória");
            }

            var alocacao = alocacaoDados.ToUpperInvariant();

            if (alocacao == "BELOW" && string.IsNullOrWhiteSpace(justificativaBelow))
            {
                return (false, "Justificativa é obrigatória quando 'Below' é selecionado");
            }

            return (true, string.Empty);
        }
        
    }
}    
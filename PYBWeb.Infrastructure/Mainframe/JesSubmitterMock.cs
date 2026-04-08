namespace PYBWeb.Infrastructure.Mainframe
{
    /// <summary>
    /// Versão MOCK para testes SEM conexão ao mainframe
    /// Simula o comportamento da DLL pxuotrxw
    /// </summary>
    public class JesSubmitterMock
    {
        public static (bool sucesso, string mensagem, string codigoRetorno, string numeroJob) SubmeterJCL(
            string caminhoJclLocal)
        {
            // Simula validações
            if (! File.Exists(caminhoJclLocal))
            {
                return (false, "❌ [MOCK] Arquivo JCL não encontrado", "82", "");
            }

            var fileInfo = new FileInfo(caminhoJclLocal);
            if (fileInfo.Length == 0)
            {
                return (false, "❌ [MOCK] Arquivo JCL vazio", "A0", "");
            }

            // Simula delay de rede (para parecer real)
            Thread.Sleep(1500);

            // Simula sucesso com número de JOB fake
            string numeroJobFake = $"JOB12345";
            string nomeArquivo = Path.GetFileName(caminhoJclLocal);
            
            string mensagem = $"✅ [MODO TESTE - MOCK]\n\n" +
                            $"JCL NÃO foi submetido ao mainframe.\n" +
                            $"Arquivo gerado: {nomeArquivo}\n" +
                            $"JOB simulado: {numeroJobFake}\n\n";

            return (true, mensagem, "00", numeroJobFake);
        }

    }
}
using PYBWeb.Domain.Entities;

namespace PYBWeb.Domain.Interfaces
{
    public interface IJclGeneratorService
    {
        /// <summary>
        /// Gera um único JCL consolidado para uma ou múltiplas solicitações (DEFINE, ALTER, DELETE)
        /// </summary>
        string GerarJclGrupo(List<SolicitacaoCics2025> grupo, AmbienteCics ambiente);
    }
}
using System.Collections.Concurrent;

namespace PYBWeb.Web.Authorization
{
    /// <summary>
    /// Serviço para preservar autenticação do usuário durante a conexão WebSocket do Blazor
    /// </summary>
    public interface IBlazorAuthenticationService
    {
        void SetAuthenticatedUser(string circuitId, string userName);
        string? GetAuthenticatedUser(string circuitId);
        void RemoveAuthenticatedUser(string circuitId);
    }

    public class BlazorAuthenticationService : IBlazorAuthenticationService
    {
        private readonly ConcurrentDictionary<string, string> _circuitUsers = 
            new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Armazena o usuário autenticado para um circuito específico
        /// </summary>
        public void SetAuthenticatedUser(string circuitId, string userName)
        {
            if (string.IsNullOrEmpty(circuitId) || string.IsNullOrEmpty(userName))
                return;

            _circuitUsers[circuitId] = userName;
            Console.WriteLine($"[BLAZOR-AUTH] Usuário '{userName}' armazenado para circuito '{circuitId}'");
        }

        /// <summary>
        /// Recupera o usuário autenticado para um circuito específico
        /// </summary>
        public string? GetAuthenticatedUser(string circuitId)
        {
            if (string.IsNullOrEmpty(circuitId))
                return null;

            var found = _circuitUsers.TryGetValue(circuitId, out var userName);
            if (found && !string.IsNullOrEmpty(userName))
            {
                Console.WriteLine($"[BLAZOR-AUTH] Usuário recuperado do circuito '{circuitId}': '{userName}'");
                return userName;
            }

            return null;
        }

        /// <summary>
        /// Remove o armazenamento de usuário quando o circuito é desconectado
        /// </summary>
        public void RemoveAuthenticatedUser(string circuitId)
        {
            if (string.IsNullOrEmpty(circuitId))
                return;

            _circuitUsers.TryRemove(circuitId, out _);
            Console.WriteLine($"[BLAZOR-AUTH] Dados de autenticação removidos para circuito '{circuitId}'");
        }
    }
}

using Microsoft.AspNetCore.Components.Server.Circuits;
using System.Security.Claims;

namespace PYBWeb.Web.Authorization
{
    /// <summary>
    /// CircuitHandler que captura o contexto de autenticação na inicialização do Blazor
    /// </summary>
    public class AuthenticationCircuitHandler : CircuitHandler
    {
        private readonly IBlazorAuthenticationService _blazorAuthService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationCircuitHandler(
            IBlazorAuthenticationService blazorAuthService, 
            IHttpContextAccessor httpContextAccessor)
        {
            _blazorAuthService = blazorAuthService ?? throw new ArgumentNullException(nameof(blazorAuthService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Chamado quando um novo circuito é criado - captura o usuário autenticado
        /// </summary>
        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[CIRCUIT] Circuito aberto: {circuit.Id}");

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var userName = httpContext.User.Identity.Name;
                
                if (!string.IsNullOrEmpty(userName))
                {
                    Console.WriteLine($"[CIRCUIT] Armazenando usuário autenticado: '{userName}'");
                    _blazorAuthService.SetAuthenticatedUser(circuit.Id, userName);
                }
                else
                {
                    // Tentar outras claims como fallback
                    var upn = httpContext.User.FindFirst(ClaimTypes.Upn)?.Value;
                    var preferred = httpContext.User.FindFirst("preferred_username")?.Value;
                    var claimName = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;

                    var resolvedUser = upn ?? preferred ?? claimName;
                    if (!string.IsNullOrEmpty(resolvedUser))
                    {
                        Console.WriteLine($"[CIRCUIT] Armazenando usuário de claim alternativo: '{resolvedUser}'");
                        _blazorAuthService.SetAuthenticatedUser(circuit.Id, resolvedUser);
                    }
                }
            }
            else
            {
                Console.WriteLine("[CIRCUIT] Nenhum usuário autenticado no contexto HTTP");
            }

            return base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }

        /// <summary>
        /// Chamado quando um circuito é desconectado - limpa o armazenamento de usuário
        /// </summary>
        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[CIRCUIT] Circuito fechado: {circuit.Id}");
            _blazorAuthService.RemoveAuthenticatedUser(circuit.Id);
            return base.OnCircuitClosedAsync(circuit, cancellationToken);
        }
    }
}

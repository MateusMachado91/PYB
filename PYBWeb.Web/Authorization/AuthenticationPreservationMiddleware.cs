namespace PYBWeb.Web.Authorization
{
    /// <summary>
    /// Middleware que garante que o usuário autenticado esteja disponível no HttpContext
    /// mesmo durante conexões WebSocket do Blazor
    /// </summary>
    public class AuthenticationPreservationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IBlazorAuthenticationService _blazorAuthService;

        public AuthenticationPreservationMiddleware(RequestDelegate next, IBlazorAuthenticationService blazorAuthService)
        {
            _next = next;
            _blazorAuthService = blazorAuthService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Se o usuário não está autenticado, tentar obter de tentativas anteriores
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                // Verificar se há um usuário armazenado na sessão
                var sessionUserKey = "AuthenticatedUser";
                if (context.Session.TryGetValue(sessionUserKey, out var userBytes))
                {
                    var userName = System.Text.Encoding.UTF8.GetString(userBytes);
                    Console.WriteLine($"[MIDDLEWARE] Usuário recuperado da sessão: {userName}");
                    context.Items["AuthenticatedUser"] = userName;
                }
            }
            else if (context.User?.Identity?.Name != null)
            {
                // Armazenar usuário autenticado na sessão para requisições futuras
                context.Session.SetString("AuthenticatedUser", context.User.Identity.Name);
                context.Items["AuthenticatedUser"] = context.User.Identity.Name;
                Console.WriteLine($"[MIDDLEWARE] Usuário armazenado na sessão: {context.User.Identity.Name}");
            }

            await _next(context);
        }
    }
}

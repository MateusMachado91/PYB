using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.Configuration;
using PYBWeb.Domain.Interfaces;
using PYBWeb.Web.Authorization;

public class UserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly IBlazorAuthenticationService? _blazorAuthService;

    public UserService(
        IHttpContextAccessor httpContextAccessor, 
        IConfiguration configuration,
        IBlazorAuthenticationService? blazorAuthService = null)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _blazorAuthService = blazorAuthService;
    }

    private const string LAST_USER_ENV_VAR = "PYBWEB_LAST_AUTHENTICATED_USER";

    public string UserName
    {
        get
        {
            string? userName = null;

            var ctx = _httpContextAccessor.HttpContext;
            if (ctx == null)
            {
                Console.WriteLine("[DEBUG] HttpContext é NULL - Rodando como Serviço");
                
                // ✅ Tentar recuperar o último usuário autenticado da variável de ambiente
                var lastUser = Environment.GetEnvironmentVariable(LAST_USER_ENV_VAR);
                if (!string.IsNullOrEmpty(lastUser))
                {
                    Console.WriteLine($"[DEBUG] Último usuário autenticado recuperado: {lastUser}");
                    return RemoveDomainPrefix(lastUser);
                }

                // Se não houver último usuário salvo, usar valor padrão
                var defaultServiceUser = _configuration["ServiceSettings:DefaultServiceUser"] ?? "SISTEMA";
                Console.WriteLine($"[DEBUG] Nenhum usuário anterior. Usando padrão: {defaultServiceUser}");
                return defaultServiceUser;
            }

            Console.WriteLine($"[DEBUG] HttpContext.User.Identity.IsAuthenticated: {ctx.User?.Identity?.IsAuthenticated}");
            Console.WriteLine($"[DEBUG] HttpContext.User.Identity.Name: '{ctx.User?.Identity?.Name}'");

            if (ctx.User?.Identity?.IsAuthenticated == true)
            {
                userName = ctx.User.Identity?.Name;
                Console.WriteLine($"[DEBUG] Identity.Name: '{userName}'");

                if (string.IsNullOrEmpty(userName))
                {
                    var upn = ctx.User.FindFirst(ClaimTypes.Upn)?.Value;
                    var preferred = ctx.User.FindFirst("preferred_username")?.Value;
                    var claimName = ctx.User.FindFirst(ClaimTypes.Name)?.Value;

                    Console.WriteLine($"[DEBUG] ClaimTypes.Upn: '{upn}'");
                    Console.WriteLine($"[DEBUG] preferred_username: '{preferred}'");
                    Console.WriteLine($"[DEBUG] ClaimTypes.Name: '{claimName}'");

                    userName = upn ?? preferred ?? claimName;
                }

                // ✅ Salvar o usuário autenticado em uma variável de ambiente
                if (!string.IsNullOrEmpty(userName))
                {
                    try
                    {
                        Environment.SetEnvironmentVariable(LAST_USER_ENV_VAR, userName, EnvironmentVariableTarget.User);
                        Console.WriteLine($"[DEBUG] Usuário '{userName}' salvo na variável de ambiente para uso futuro");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[DEBUG] Erro ao salvar usuário em variável de ambiente: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("[DEBUG] Usuário NÃO autenticado no HttpContext.");
                
                // 🔄 Tentar recuperar do HttpContext.Items (armazenado pelo middleware)
                if (ctx.Items.TryGetValue("AuthenticatedUser", out var itemUser) && itemUser is string itemUserStr)
                {
                    Console.WriteLine($"[DEBUG] Usuário encontrado em HttpContext.Items: '{itemUserStr}'");
                    userName = itemUserStr;
                }
                // 🔄 Tentar recuperar do serviço de autenticação do Blazor (quando em conexão WebSocket)
                else if (_blazorAuthService != null && ctx.Items.TryGetValue("circuitId", out var circuitIdObj) && circuitIdObj is string circuitId)
                {
                    Console.WriteLine($"[DEBUG] Tentando recuperar usuário do circuito Blazor: {circuitId}");
                    var blazorUser = _blazorAuthService.GetAuthenticatedUser(circuitId);
                    if (!string.IsNullOrEmpty(blazorUser))
                    {
                        Console.WriteLine($"[DEBUG] Usuário recuperado do armazenamento Blazor: '{blazorUser}'");
                        userName = blazorUser;
                    }
                    else
                    {
                        Console.WriteLine("[DEBUG] Nenhum usuário encontrado no armazenamento Blazor.");
                    }
                }
            }

            // Se ainda não temos usuário, tentar a variável de ambiente
            if (string.IsNullOrEmpty(userName))
            {
                var lastUser = Environment.GetEnvironmentVariable(LAST_USER_ENV_VAR);
                if (!string.IsNullOrEmpty(lastUser))
                {
                    Console.WriteLine($"[DEBUG] Usuário não autenticado, usando último salvo: {lastUser}");
                    userName = lastUser;
                }
            }

            Console.WriteLine($"[DEBUG] Valor final de userName: '{userName}'");
            return userName ?? string.Empty;
        }
        set
        {
            // setter sem efeito
        }
    }


    // Remove domínio se existir (ex: "CORP\usuario" -> "usuario")
    public string UserNameSemDominio => RemoveDomainPrefix(UserName);

    public string GetCurrentUser() => UserName;

    private static string RemoveDomainPrefix(string name)
    {
        if (string.IsNullOrEmpty(name)) return string.Empty;

        // trata formatos "DOMINIO\usuario" e "usuario@dominio"
        var idx = name.IndexOf('\\');
        if (idx >= 0 && idx < name.Length - 1)
            return name.Substring(idx + 1);

        // se for UPN, retorna antes do @
        var atIdx = name.IndexOf('@');
        if (atIdx > 0)
            return name.Substring(0, atIdx);

        return name;
    }
}


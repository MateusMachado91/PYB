using Microsoft.AspNetCore.Authorization;
using PYBWeb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PYBWeb.Web.Authorization
{
    public class AdminRequirement : IAuthorizationRequirement { }

    public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        public AdminAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            AdminRequirement requirement)
        {
            // Obter username do contexto
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            
            // ⚠️ SIMULAÇÃO PARA TESTES - Remover em produção
        if (string.IsNullOrEmpty(userName))
        {
            context.Fail();
            return;
        }

            // Remover o prefixo CORP\ da matrícula
            var matricula = userName.Replace("CORP\\", "").ToUpper();

            // Criar scope para acessar o DbContext
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ColaboradoresDbContext>();

            // Verificar se o colaborador existe e tem role admin
            var colaborador = await dbContext.Colaboradores
                .FirstOrDefaultAsync(c => c.Matricula.ToUpper() == matricula);

            if (colaborador != null && colaborador.Role.ToLower() == "admin")
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}

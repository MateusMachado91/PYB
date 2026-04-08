namespace PYBWeb.Domain.Interfaces;

/// <summary>
/// Interface para obter informações do usuário atual
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Obtém o nome do usuário atual
    /// </summary>
    string UserName { get; set; }
    string UserNameSemDominio { get; }
    string GetCurrentUser();
}


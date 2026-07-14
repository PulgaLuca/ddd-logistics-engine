namespace Dhexby.LogisticsEngine.Api.Authorization;

/// <summary>
/// Nomi dei ruoli (devono combaciare con <c>UserRole</c> del dominio) e gruppi
/// pronti per l'attributo [Authorize(Roles = ...)].
/// </summary>
public static class Roles
{
    public const string Administrator = "Administrator";
    public const string Manager = "Manager";
    public const string Operator = "Operator";
    public const string Viewer = "Viewer";

    /// <summary>Chi può creare/gestire i dati anagrafici.</summary>
    public const string ManageData = $"{Administrator},{Manager}";

    /// <summary>Chi può eseguire azioni operative su dati esistenti.</summary>
    public const string Operate = $"{Administrator},{Manager},{Operator}";
}

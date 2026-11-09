namespace LogisticsEngine.Domain.Users.Enums;

/// <summary>Ruoli applicativi iniziali; le autorizzazioni effettive sono espresse tramite UserPermission.</summary>
public enum UserRole
{
    Viewer,
    Operator,
    Administrator
}

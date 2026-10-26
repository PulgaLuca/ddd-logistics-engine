using Dhexby.LogisticsEngine.Application.Hubs.RegisterHub;

namespace Dhexby.LogisticsEngine.Api.Controllers.Hubs
{
    /// <summary>Contratto HTTP di registrazione hub.</summary>
    public sealed record RegisterHubRequest(string Code, double Latitude, double Longitude)
    {
        public RegisterHubCommand ToCommand() => new(Code, Latitude, Longitude);
    }

}

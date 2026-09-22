using System.Reflection;

namespace LogisticsEngine.ArchitectureTests;

public abstract class ArchitectureTestBase
{
    protected static readonly Assembly SharedKernelAssembly = typeof(SharedKernel.Result).Assembly;
    protected static readonly Assembly DomainAssembly = typeof(Domain.Hubs.Hub).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(Application.DependencyInjection).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.DependencyInjection).Assembly;
    protected const string SharedKernelNamespace = "LogisticsEngine.SharedKernel";
    protected const string DomainNamespace = "LogisticsEngine.Domain";
    protected const string ApplicationNamespace = "LogisticsEngine.Application";
    protected const string InfrastructureNamespace = "LogisticsEngine.Infrastructure";
    protected const string WebApiNamespace = "LogisticsEngine.WebApi";
}

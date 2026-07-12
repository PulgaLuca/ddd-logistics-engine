using System.Reflection;

namespace Dhexby.LogisticsEngine.ArchitectureTests;

/// <summary>
/// Regole di dipendenza della Clean Architecture, verificate sui riferimenti
/// tra assembly: una violazione qui fa fallire la pipeline.
/// </summary>
public class LayerDependencyTests
{
    private static bool References(Assembly assembly, Assembly forbidden) =>
        assembly.GetReferencedAssemblies()
            .Any(reference => reference.Name == forbidden.GetName().Name);

    [Fact]
    public void Domain_ShouldNotDependOn_Application()
    {
        Assert.False(References(ArchitectureAssemblies.Domain, ArchitectureAssemblies.Application));
    }

    [Fact]
    public void Domain_ShouldNotDependOn_Infrastructure()
    {
        Assert.False(References(ArchitectureAssemblies.Domain, ArchitectureAssemblies.Infrastructure));
    }

    [Fact]
    public void Domain_ShouldNotDependOn_Api()
    {
        Assert.False(References(ArchitectureAssemblies.Domain, ArchitectureAssemblies.Api));
    }

    [Fact]
    public void Application_ShouldNotDependOn_Infrastructure()
    {
        Assert.False(References(ArchitectureAssemblies.Application, ArchitectureAssemblies.Infrastructure));
    }

    [Fact]
    public void Application_ShouldNotDependOn_Api()
    {
        Assert.False(References(ArchitectureAssemblies.Application, ArchitectureAssemblies.Api));
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_Api()
    {
        Assert.False(References(ArchitectureAssemblies.Infrastructure, ArchitectureAssemblies.Api));
    }
}

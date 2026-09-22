using NetArchTest.Rules;

namespace LogisticsEngine.ArchitectureTests;

/// <summary>Verifica automaticamente la regola delle dipendenze della Clean Architecture.</summary>
public sealed class LayerDependencyTests : ArchitectureTestBase
{
    [Fact]
    public void Domain_ShouldNotDependOn_OtherLayers()
    {
        NetArchTest.Rules.TestResult result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, WebApiNamespace)
            .GetResult();

        result.FailingTypeNames.ShouldBeNull();
    }

    [Fact]
    public void SharedKernel_ShouldNotDependOn_AnyProjectLayer()
    {
        NetArchTest.Rules.TestResult result = Types.InAssembly(SharedKernelAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(DomainNamespace, ApplicationNamespace, InfrastructureNamespace, WebApiNamespace)
            .GetResult();

        result.FailingTypeNames.ShouldBeNull();
    }

    [Fact]
    public void Application_ShouldNotDependOn_InfrastructureOrWebApi()
    {
        NetArchTest.Rules.TestResult result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespace, WebApiNamespace)
            .GetResult();

        result.FailingTypeNames.ShouldBeNull();
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOn_WebApi()
    {
        NetArchTest.Rules.TestResult result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOn(WebApiNamespace)
            .GetResult();

        result.FailingTypeNames.ShouldBeNull();
    }

    [Fact]
    public void Domain_ShouldNotDependOn_EntityFrameworkOrDrivers()
    {
        NetArchTest.Rules.TestResult result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "MongoDB", "StackExchange.Redis", "Npgsql")
            .GetResult();

        result.FailingTypeNames.ShouldBeNull();
    }
}


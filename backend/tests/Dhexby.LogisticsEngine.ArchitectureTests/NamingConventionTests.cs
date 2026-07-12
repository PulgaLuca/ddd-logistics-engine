using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dhexby.LogisticsEngine.ArchitectureTests;

/// <summary>
/// Convenzioni automatizzabili: suffissi di Command/Query/Handler,
/// posizione dei repository concreti, dipendenze dei controller.
/// </summary>
public class NamingConventionTests
{
    [Fact]
    public void RequestHandlers_ShouldEndWith_Handler()
    {
        IEnumerable<Type> handlers = ArchitectureAssemblies.Application.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .Where(type => type.GetInterfaces().Any(iface =>
                iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)));

        Assert.All(handlers, handler => Assert.EndsWith("Handler", handler.Name, StringComparison.Ordinal));
    }

    [Fact]
    public void Requests_ShouldEndWith_CommandOrQuery()
    {
        IEnumerable<Type> requests = ArchitectureAssemblies.Application.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .Where(type => typeof(IBaseRequest).IsAssignableFrom(type));

        Assert.All(requests, request => Assert.True(
            request.Name.EndsWith("Command", StringComparison.Ordinal) ||
            request.Name.EndsWith("Query", StringComparison.Ordinal),
            $"{request.Name} must end with 'Command' or 'Query'."));
    }

    [Fact]
    public void ConcreteRepositories_ShouldLiveIn_Infrastructure()
    {
        Type[] repositoryContracts = ArchitectureAssemblies.Domain.GetTypes()
            .Where(type => type.IsInterface && type.Name.EndsWith("Repository", StringComparison.Ordinal))
            .ToArray();

        Assembly[] productionAssemblies =
        [
            ArchitectureAssemblies.Domain,
            ArchitectureAssemblies.Application,
            ArchitectureAssemblies.Infrastructure,
            ArchitectureAssemblies.Api,
        ];

        IEnumerable<Type> concreteRepositories = productionAssemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .Where(type => repositoryContracts.Any(contract => contract.IsAssignableFrom(type)));

        Assert.All(concreteRepositories, repository =>
            Assert.Equal(ArchitectureAssemblies.Infrastructure, repository.Assembly));
    }

    [Fact]
    public void Controllers_ShouldNotDependOn_Repositories()
    {
        IEnumerable<Type> controllers = ArchitectureAssemblies.Api.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type));

        foreach (Type controller in controllers)
        {
            IEnumerable<ParameterInfo> constructorParameters = controller
                .GetConstructors()
                .SelectMany(constructor => constructor.GetParameters());

            Assert.All(constructorParameters, parameter => Assert.False(
                parameter.ParameterType.Name.EndsWith("Repository", StringComparison.Ordinal),
                $"{controller.Name} must depend on the Application layer (MediatR), not on repositories."));
        }
    }

    [Fact]
    public void Controllers_ShouldEndWith_Controller()
    {
        IEnumerable<Type> controllers = ArchitectureAssemblies.Api.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type));

        Assert.All(controllers, controller =>
            Assert.EndsWith("Controller", controller.Name, StringComparison.Ordinal));
    }
}

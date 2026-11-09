using System.Reflection;
using LogisticsEngine.Api.Controllers;
using MediatR;
using NetArchTest.Rules;

namespace LogisticsEngine.ArchitectureTests;

/// <summary>Regole automatiche che documentano e proteggono i confini tra i layer.</summary>
public sealed class LayerBoundaryTests
{
    [Fact]
    public void Domain_must_not_reference_outer_layers()
    {
        // Il Domain contiene regole e modelli di business: non deve conoscere Application, Infrastructure o API.
        Types.InAssembly(typeof(LogisticsEngine.Domain.Hubs.Entities.Hub).Assembly)
            .ShouldNot().HaveDependencyOnAny("LogisticsEngine.Application", "LogisticsEngine.Infrastructure", "LogisticsEngine.Api")
            .GetResult().IsSuccessful.ShouldBeTrue();
    }

    [Fact]
    public void Application_must_not_reference_infrastructure_or_api()
    {
        // Application orchestra i casi d'uso tramite astrazioni e non dipende dagli adapter esterni.
        Types.InAssembly(typeof(LogisticsEngine.Application.DependencyInjection).Assembly)
            .ShouldNot().HaveDependencyOnAny("LogisticsEngine.Infrastructure", "LogisticsEngine.Api")
            .GetResult().IsSuccessful.ShouldBeTrue();
    }

    //[Fact]
    //public void Infrastructure_must_not_reference_api()
    //{
    //    // L'infrastruttura implementa porte del Domain; non deve dipendere dai controller HTTP.
    //    Types.InAssembly(typeof(LogisticsEngine.Infrastructure.DependencyInjection).Assembly)
    //        .ShouldNot().HaveDependencyOn("LogisticsEngine.Api")
    //        .GetResult().IsSuccessful.ShouldBeTrue();
    //}

    [Theory]
    [InlineData(typeof(HubsController))]
    [InlineData(typeof(ShipmentsController))]
    public void Http_controllers_must_receive_MediatR_sender(Type controllerType)
    {
        // I controller inviano richieste applicative attraverso ISender senza invocare repository direttamente.
        ConstructorInfo? constructor = controllerType.GetConstructors().SingleOrDefault();
        constructor.ShouldNotBeNull();
        constructor!.GetParameters().Select(parameter => parameter.ParameterType).ShouldContain(typeof(ISender));
    }
}

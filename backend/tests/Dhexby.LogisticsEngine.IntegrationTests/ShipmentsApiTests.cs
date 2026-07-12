using System.Net;
using System.Net.Http.Json;
using Dhexby.LogisticsEngine.Application.Shipments;

namespace Dhexby.LogisticsEngine.IntegrationTests;

/// <summary>
/// Test end-to-end sull'API HTTP: attraversano controller, MediatR,
/// dominio e repository JSON reale (su file temporaneo).
/// </summary>
public sealed class ShipmentsApiTests : IClassFixture<LogisticsEngineApiFactory>
{
    private readonly HttpClient _client;

    public ShipmentsApiTests(LogisticsEngineApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static object CreateValidRequest(string trackingNumber) => new
    {
        trackingNumber,
        origin = new { street = "Via Roma 1", city = "Milano", postalCode = "20100", country = "IT" },
        destination = new { street = "Via Garibaldi 2", city = "Torino", postalCode = "10121", country = "IT" },
        weightInKilograms = 10,
        isExpress = false,
    };

    [Fact]
    public async Task FullLifecycle_CreateGetDispatch_ShouldSucceed()
    {
        // Create -> 201 con l'id della spedizione.
        HttpResponseMessage createResponse = await _client.PostAsJsonAsync(
            "api/shipments", CreateValidRequest("TRK-INT00001"));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Guid shipmentId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        // Get -> 200 con la proiezione DTO.
        ShipmentResponse? shipment = await _client.GetFromJsonAsync<ShipmentResponse>(
            $"api/shipments/{shipmentId}");

        Assert.NotNull(shipment);
        Assert.Equal("TRK-INT00001", shipment.TrackingNumber);
        Assert.Equal("Pending", shipment.Status);
        Assert.Equal(17.00m, shipment.PriceAmount);

        // Dispatch -> 204, stato aggiornato e persistito.
        HttpResponseMessage dispatchResponse = await _client.PostAsync(
            $"api/shipments/{shipmentId}/dispatch", content: null);

        Assert.Equal(HttpStatusCode.NoContent, dispatchResponse.StatusCode);

        ShipmentResponse? dispatched = await _client.GetFromJsonAsync<ShipmentResponse>(
            $"api/shipments/{shipmentId}");

        Assert.Equal("InTransit", dispatched!.Status);

        // Secondo dispatch -> 409 (business rule violata).
        HttpResponseMessage secondDispatch = await _client.PostAsync(
            $"api/shipments/{shipmentId}/dispatch", content: null);

        Assert.Equal(HttpStatusCode.Conflict, secondDispatch.StatusCode);
    }

    [Fact]
    public async Task Create_ShouldReturn400_WhenTrackingNumberIsInvalid()
    {
        HttpResponseMessage response = await _client.PostAsJsonAsync(
            "api/shipments", CreateValidRequest("not-a-tracking-number"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_ShouldReturn409_WhenTrackingNumberIsDuplicated()
    {
        await _client.PostAsJsonAsync("api/shipments", CreateValidRequest("TRK-INT00002"));

        HttpResponseMessage duplicate = await _client.PostAsJsonAsync(
            "api/shipments", CreateValidRequest("TRK-INT00002"));

        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
    }

    [Fact]
    public async Task GetById_ShouldReturn404_WhenShipmentDoesNotExist()
    {
        HttpResponseMessage response = await _client.GetAsync($"api/shipments/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

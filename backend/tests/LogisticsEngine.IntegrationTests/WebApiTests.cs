using System.Net;
using System.Net.Http.Json;
using LogisticsEngine.Application.Hubs;
using LogisticsEngine.Domain.Hubs;
using LogisticsEngine.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LogisticsEngine.IntegrationTests;

public sealed class WebApiTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "LogisticsEngineTests", Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task Crud_round_trip_persists_json_and_handles_missing_hubs()
    {
        using var factory = CreateFactory();
        using HttpClient client = factory.CreateClient();
        (await client.GetFromJsonAsync<HubDto[]>("/api/hubs", TestContext.Current.CancellationToken)).ShouldBeEmpty();
        using HttpResponseMessage created = await client.PostAsJsonAsync("/api/hubs", new { name = " Milano ", zone = " nord " }, TestContext.Current.CancellationToken);
        created.StatusCode.ShouldBe(HttpStatusCode.Created);
        HubDto hub = (await created.Content.ReadFromJsonAsync<HubDto>(TestContext.Current.CancellationToken))!;
        hub.Name.ShouldBe("Milano");
        hub.Zone.ShouldBe("NORD");
        created.Headers.Location.ShouldNotBeNull();
        (await client.GetFromJsonAsync<HubDto>(created.Headers.Location, TestContext.Current.CancellationToken)).ShouldBe(hub);
        (await client.GetFromJsonAsync<HubDto[]>("/api/hubs", TestContext.Current.CancellationToken))!.ShouldHaveSingleItem().ShouldBe(hub);

        // Una nuova istanza legge realmente dal disco, senza una cache in memoria.
        var repository = new JsonHubRepository(_directory);
        (await repository.GetByIdAsync(hub.Id, TestContext.Current.CancellationToken))!.Name.ShouldBe("Milano");
        Directory.GetFiles(_directory, "*.json").Length.ShouldBe(1);

        using HttpResponseMessage updated = await client.PutAsJsonAsync($"/api/hubs/{hub.Id}", new { name = "Roma", zone = "CENTRO" }, TestContext.Current.CancellationToken);
        updated.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await repository.GetByIdAsync(hub.Id, TestContext.Current.CancellationToken))!.Name.ShouldBe("Roma");

        using HttpResponseMessage deleted = await client.DeleteAsync($"/api/hubs/{hub.Id}", TestContext.Current.CancellationToken);
        deleted.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await client.GetAsync($"/api/hubs/{hub.Id}", TestContext.Current.CancellationToken)).StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await client.PutAsJsonAsync($"/api/hubs/{hub.Id}", new { name = "Roma", zone = "SUD" }, TestContext.Current.CancellationToken)).StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await client.DeleteAsync($"/api/hubs/{hub.Id}", TestContext.Current.CancellationToken)).StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await client.GetFromJsonAsync<HubDto[]>("/api/hubs", TestContext.Current.CancellationToken)).ShouldBeEmpty();
    }

    [Theory]
    [InlineData("", "NORD")]
    [InlineData("Milano", "1")]
    [InlineData(null, null)]
    public async Task Invalid_create_returns_400_without_saving(string? name, string? zone)
    {
        using var factory = CreateFactory();
        using HttpClient client = factory.CreateClient();
        using HttpResponseMessage response = await client.PostAsJsonAsync("/api/hubs", new { name, zone }, TestContext.Current.CancellationToken);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await client.GetFromJsonAsync<HubDto[]>("/api/hubs", TestContext.Current.CancellationToken)).ShouldBeEmpty();
    }

    [Fact]
    public async Task Invalid_update_preserves_persisted_data()
    {
        using var factory = CreateFactory();
        using HttpClient client = factory.CreateClient();
        using HttpResponseMessage created = await client.PostAsJsonAsync("/api/hubs", new { name = "Milano", zone = "NORD" }, TestContext.Current.CancellationToken);
        HubDto hub = (await created.Content.ReadFromJsonAsync<HubDto>(TestContext.Current.CancellationToken))!;
        (await client.PutAsJsonAsync($"/api/hubs/{hub.Id}", new { name = "Roma", zone = "1" }, TestContext.Current.CancellationToken)).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await client.GetFromJsonAsync<HubDto>($"/api/hubs/{hub.Id}", TestContext.Current.CancellationToken)).ShouldBe(hub);
    }

    [Fact]
    public async Task Repository_rejects_duplicates_and_handles_missing_records()
    {
        var repository = new JsonHubRepository(_directory);
        Hub hub = Hub.Create(Guid.NewGuid(), "Milano", "NORD").Value;
        (await repository.GetByIdAsync(hub.Id, TestContext.Current.CancellationToken)).ShouldBeNull();
        (await repository.UpdateAsync(hub, TestContext.Current.CancellationToken)).ShouldBeFalse();
        (await repository.DeleteAsync(hub.Id, TestContext.Current.CancellationToken)).ShouldBeFalse();
        await repository.AddAsync(hub, TestContext.Current.CancellationToken);
        await Should.ThrowAsync<IOException>(() => repository.AddAsync(hub, TestContext.Current.CancellationToken));
        (await repository.GetAllAsync(TestContext.Current.CancellationToken)).ShouldHaveSingleItem();
        Directory.GetFiles(_directory, "*.tmp").ShouldBeEmpty();
    }

    [Theory]
    [InlineData("null")]
    [InlineData("{}")]
    [InlineData("{broken")]
    public async Task Repository_does_not_silently_ignore_corrupt_data(string json)
    {
        Directory.CreateDirectory(_directory);
        Guid id = Guid.NewGuid();
        await File.WriteAllTextAsync(Path.Combine(_directory, $"{id:D}.json"), json, TestContext.Current.CancellationToken);
        var repository = new JsonHubRepository(_directory);
        await Should.ThrowAsync<Exception>(() => repository.GetByIdAsync(id, TestContext.Current.CancellationToken));
    }

    private WebApplicationFactory<Program> CreateFactory() =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder => builder.UseSetting("DataDirectory", _directory));

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }
}

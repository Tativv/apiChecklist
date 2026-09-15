using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using HotelChecklist.Api.Features.Areas.Create;
using HotelChecklist.Api.Features.Assets.Create;
using HotelChecklist.Api.Features.Auth.Login;
using HotelChecklist.Api.Features.ChecklistInstances.Approve;
using HotelChecklist.Api.Features.ChecklistInstances.Finish;
using HotelChecklist.Api.Features.ChecklistInstances.GetById;
using HotelChecklist.Api.Features.ChecklistInstances.Start;
using HotelChecklist.Api.Features.ChecklistTemplates;
using HotelChecklist.Api.Features.ChecklistTemplates.ApplyToAssets;
using HotelChecklist.Api.Features.ChecklistTemplates.Create;
using HotelChecklist.IntegrationTests.Infrastructure;

namespace HotelChecklist.IntegrationTests;

public class ChecklistLifecycleTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client = null!;

    public ChecklistLifecycleTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.InitializeAsync();
        _client = _factory.CreateClient();

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest("admin@hotelchecklist.local", "Admin123!"));
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);
    }

    public async Task DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task FullChecklistLifecycle_FromCreationToApproval_ShouldSucceed()
    {
        var areaResponse = await _client.PostAsJsonAsync("/api/areas", new CreateAreaRequest($"Área {Guid.NewGuid()}"));
        areaResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var area = await areaResponse.Content.ReadFromJsonAsync<CreateAreaResponse>();

        var assetResponse = await _client.PostAsJsonAsync("/api/assets", new CreateAssetRequest("Habitación 101", "Room", area!.Id));
        assetResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var asset = await assetResponse.Content.ReadFromJsonAsync<CreateAssetResponse>();

        var templateResponse = await _client.PostAsJsonAsync("/api/checklist-templates", new CreateChecklistTemplateRequest(
            "Checklist diario",
            "Revisión diaria",
            area.Id,
            "Daily",
            30,
            [new ChecklistTaskRequest("Tender cama", 1), new ChecklistTaskRequest("Limpiar baño", 2)]));
        templateResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var template = await templateResponse.Content.ReadFromJsonAsync<CreateChecklistTemplateResponse>();

        var applyResponse = await _client.PostAsJsonAsync(
            $"/api/checklist-templates/{template!.Id}/apply-to-assets",
            new ApplyTemplateToAssetsRequest([asset!.Id], DateOnly.FromDateTime(DateTime.UtcNow), null));
        applyResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var applyResult = await applyResponse.Content.ReadFromJsonAsync<ApplyTemplateToAssetsResponse>();
        applyResult!.Created.Should().Be(1);
        var instanceId = applyResult.CreatedInstanceIds.Single();

        var startResponse = await _client.PostAsync($"/api/checklist-instances/{instanceId}/start", null);
        startResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var started = await startResponse.Content.ReadFromJsonAsync<StartChecklistInstanceResponse>();
        started!.Status.Should().Be("InProgress");

        var detail = await _client.GetFromJsonAsync<GetChecklistInstanceByIdResponse>($"/api/checklist-instances/{instanceId}");

        foreach (var taskExecution in detail!.TaskExecutions)
        {
            var completeResponse = await _client.PostAsJsonAsync(
                $"/api/checklist-instances/{instanceId}/tasks/{taskExecution.Id}/complete",
                new { Completed = true, Comment = (string?)null });
            completeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        var finishResponse = await _client.PostAsync($"/api/checklist-instances/{instanceId}/finish", null);
        finishResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var finished = await finishResponse.Content.ReadFromJsonAsync<FinishChecklistInstanceResponse>();
        finished!.Status.Should().Be("Completed");
        finished.DurationSeconds.Should().BeGreaterThanOrEqualTo(0);

        var approveResponse = await _client.PostAsync($"/api/checklist-instances/{instanceId}/approve", null);
        approveResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var approved = await approveResponse.Content.ReadFromJsonAsync<ApproveChecklistInstanceResponse>();
        approved!.Status.Should().Be("Approved");
    }
}

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using HotelChecklist.Api.Features.Areas.Create;
using HotelChecklist.Api.Features.Assets.Create;
using HotelChecklist.Api.Features.Auth.Login;
using HotelChecklist.Api.Features.ChecklistInstances.Create;
using HotelChecklist.Api.Features.ChecklistInstances.Finish;
using HotelChecklist.Api.Features.ChecklistInstances.GetById;
using HotelChecklist.Api.Features.ChecklistTemplates;
using HotelChecklist.Api.Features.ChecklistTemplates.ConfigureAssets;
using HotelChecklist.Api.Features.ChecklistTemplates.Create;
using HotelChecklist.IntegrationTests.Infrastructure;

namespace HotelChecklist.IntegrationTests;

public class ChecklistLifecycleTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private HttpClient _client = null!;
    private Guid _gerenciaUserId;

    public ChecklistLifecycleTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.InitializeAsync();
        _client = _factory.CreateClient();

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest("gerencia@hotelchecklist.local", "Gerencia123!"));
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);
        _gerenciaUserId = login!.UserId;
    }

    public async Task DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task FullChecklistLifecycle_FromCreationToCompletion_ShouldSucceed()
    {
        var areaResponse = await _client.PostAsJsonAsync("/api/areas", new CreateAreaRequest($"Área {Guid.NewGuid()}"));
        areaResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var area = await areaResponse.Content.ReadFromJsonAsync<CreateAreaResponse>();

        var assetResponse = await _client.PostAsJsonAsync("/api/assets", new CreateAssetRequest("Habitación 101", "Room", area!.Id));
        assetResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var asset = await assetResponse.Content.ReadFromJsonAsync<CreateAssetResponse>();

        var dailySchedule = new ScheduleInput("Daily", 1, null, null, "08:00", 1);
        var templateResponse = await _client.PostAsJsonAsync("/api/checklist-templates", new CreateChecklistTemplateRequest(
            "Checklist diario",
            "Revisión diaria",
            area.Id,
            "Scheduled",
            [dailySchedule],
            [
                new ChecklistTaskRequest("Tender cama", null, 1, 15, "Scheduled", [dailySchedule]),
                new ChecklistTaskRequest("Limpiar baño", null, 2, 10, "Scheduled", [dailySchedule])
            ]));
        templateResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var template = await templateResponse.Content.ReadFromJsonAsync<CreateChecklistTemplateResponse>();

        var configureAssetsResponse = await _client.PutAsJsonAsync(
            $"/api/checklist-templates/{template!.Id}/assets",
            new ConfigureTemplateAssetsRequest([asset!.Id]));
        configureAssetsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var configureAssetsResult = await configureAssetsResponse.Content.ReadFromJsonAsync<ConfigureTemplateAssetsResponse>();
        configureAssetsResult!.AssetCount.Should().Be(1);

        var createInstanceResponse = await _client.PostAsJsonAsync(
            "/api/checklist-instances",
            new CreateChecklistInstanceRequest(template.Id, asset.Id, DateOnly.FromDateTime(DateTime.UtcNow)));
        createInstanceResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdInstance = await createInstanceResponse.Content.ReadFromJsonAsync<CreateChecklistInstanceResponse>();
        var instanceId = createdInstance!.Id;

        var pendingDetail = await _client.GetFromJsonAsync<GetChecklistInstanceByIdResponse>($"/api/checklist-instances/{instanceId}");
        pendingDetail!.Status.Should().Be("Pending");

        var isFirstAssignment = true;
        foreach (var taskExecution in pendingDetail.TaskExecutions)
        {
            var assignResponse = await _client.PostAsJsonAsync(
                $"/api/checklist-instances/{instanceId}/tasks/{taskExecution.Id}/assign",
                new { UserId = _gerenciaUserId });
            assignResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            if (isFirstAssignment)
            {
                var afterFirstAssign = await _client.GetFromJsonAsync<GetChecklistInstanceByIdResponse>($"/api/checklist-instances/{instanceId}");
                afterFirstAssign!.Status.Should().Be("InProgress", "la primera asignación arranca el checklist entero automáticamente");
                isFirstAssignment = false;
            }
        }

        var inProgressDetail = await _client.GetFromJsonAsync<GetChecklistInstanceByIdResponse>($"/api/checklist-instances/{instanceId}");

        foreach (var taskExecution in inProgressDetail!.TaskExecutions)
        {
            var startTaskResponse = await _client.PostAsync($"/api/checklist-instances/{instanceId}/tasks/{taskExecution.Id}/start", null);
            startTaskResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var completeResponse = await _client.PostAsJsonAsync(
                $"/api/checklist-instances/{instanceId}/tasks/{taskExecution.Id}/complete",
                new { Comment = (string?)null });
            completeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var reviewResponse = await _client.PostAsync($"/api/checklist-instances/{instanceId}/tasks/{taskExecution.Id}/review", null);
            reviewResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        var finishResponse = await _client.PostAsync($"/api/checklist-instances/{instanceId}/finish", null);
        finishResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var finished = await finishResponse.Content.ReadFromJsonAsync<FinishChecklistInstanceResponse>();
        finished!.Status.Should().Be("Completed");
        finished.DurationSeconds.Should().BeGreaterThanOrEqualTo(0);
    }
}

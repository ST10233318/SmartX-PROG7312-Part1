using Microsoft.AspNetCore.Mvc;
namespace SmartX.Client.Controllers;
public class RealtimeController(IHttpClientFactory clients) : Controller
{
    public IActionResult Index() => View();
    [HttpGet]
    public async Task<IActionResult> State([FromQuery] string? category, [FromQuery] bool alertsOnly = false, [FromQuery] string? search = null)
        => await Proxy(HttpMethod.Get, $"api/realtime/state?category={Uri.EscapeDataString(category ?? "")}&alertsOnly={alertsOnly}&search={Uri.EscapeDataString(search ?? "")}");
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Command([FromBody] CommandInput input) => await Proxy(HttpMethod.Post, "api/realtime/commands", input);
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Undo() => await Proxy(HttpMethod.Post, "api/realtime/undo");
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessNext() => await Proxy(HttpMethod.Post, "api/realtime/process-next");
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Simulate([FromBody] SimulationInput input) => await Proxy(HttpMethod.Post, "api/realtime/simulate", input);
    private async Task<IActionResult> Proxy(HttpMethod method, string path, object? payload = null)
    {
        try
        {
            using var request = new HttpRequestMessage(method, path);
            if (payload is not null) request.Content = System.Net.Http.Json.JsonContent.Create(payload);
            using var response = await clients.CreateClient("SmartXApi").SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return new ContentResult { Content = content, ContentType = "application/json", StatusCode = (int)response.StatusCode };
        }
        catch (HttpRequestException) { return StatusCode(503, new { error = "API unavailable. Start SmartX.Api first." }); }
    }
}
public record CommandInput(string DeviceId, string Action);
public record SimulationInput(string DeviceId, string Category, double Value, bool Critical);

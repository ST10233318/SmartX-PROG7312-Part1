using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
namespace SmartX.Client.Controllers;
[ApiController]
[Route("proxy")]
public sealed class ApiProxyController(IHttpClientFactory factory) : ControllerBase
{
    private HttpClient Api() => factory.CreateClient("SmartXApi");
    [HttpGet("topology")]
    public async Task<IActionResult> Topology() => await Forward("api/topology");
    [HttpGet("route/{id}")]
    public async Task<IActionResult> Route(string id) => await Forward("api/topology/route/" + Uri.EscapeDataString(id));
    [HttpGet("history")]
    public async Task<IActionResult> History([FromQuery] DateTime from, [FromQuery] DateTime to,
        [FromQuery] string? deviceId)
    {
        var q = $"api/history?from={Uri.EscapeDataString(from.ToString("O"))}&to={Uri.EscapeDataString(to.ToString("O"))}";
        if (!string.IsNullOrWhiteSpace(deviceId)) q += "&deviceId=" + Uri.EscapeDataString(deviceId);
        return await Forward(q);
    }
    [HttpGet("commands")]
    public async Task<IActionResult> Commands() => await Forward("api/commands");
    [HttpPost("commands")]
    public async Task<IActionResult> Send([FromBody] System.Text.Json.JsonElement body)
    {
        try
        {
            var response = await Api().PostAsJsonAsync("api/commands", body);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (HttpRequestException) { return StatusCode(503, "API unavailable"); }
    }
    [HttpPost("connections")]
    public async Task<IActionResult> Connect([FromBody] System.Text.Json.JsonElement body)
    {
        try
        {
            var response = await Api().PostAsJsonAsync("api/topology/connections", body);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (HttpRequestException) { return StatusCode(503, "API unavailable"); }
    }
    private async Task<IActionResult> Forward(string path)
    {
        try
        {
            var response = await Api().GetAsync(path);
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (HttpRequestException) { return StatusCode(503, "API unavailable"); }
    }
}

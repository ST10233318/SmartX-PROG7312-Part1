using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/topology")]
public sealed class TopologyController(TopologyService topology) : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromQuery] string gateway = "Gateway")
    {
        try { return Ok(topology.Snapshot(gateway)); }
        catch (ArgumentException e) { return BadRequest(e.Message); }
    }
    public sealed record ConnectionRequest(string From, string To, double Weight);
    [HttpPost("connections")]
    public IActionResult Connect(ConnectionRequest request)
    {
        try { topology.AddConnection(request.From, request.To, request.Weight); return Ok(request); }
        catch (ArgumentException e) { return BadRequest(e.Message); }
    }
    [HttpPost("nodes/{id}")]
    public IActionResult AddNode(string id)
    {
        try { topology.AddNode(id); return Ok(new { id }); }
        catch (ArgumentException e) { return BadRequest(e.Message); }
    }
    [HttpGet("route/{deviceId}")]
    public IActionResult Route(string deviceId, [FromQuery] string gateway = "Gateway")
    {
        try { return Ok(new { path = topology.Route(gateway, deviceId) }); }
        catch (ArgumentException e) { return BadRequest(e.Message); }
    }
}

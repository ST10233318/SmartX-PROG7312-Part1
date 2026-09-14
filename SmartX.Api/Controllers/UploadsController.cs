using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Data;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadsController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;

    public UploadsController(
        IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    [HttpPost("{deviceId}")]
    public async Task<IActionResult> Upload(
        string deviceId,
        IFormFile file)
    {
        if (!SmartXStore.Sensors.TryGetValue(
            deviceId,
            out var sensor))
        {
            return NotFound("Sensor not found.");
        }

        if (file == null || file.Length == 0)
        {
            return BadRequest("Please select a file.");
        }

        var uploadsFolder = Path.Combine(
            _environment.WebRootPath ?? "wwwroot",
            "uploads");

        Directory.CreateDirectory(uploadsFolder);

        var safeFileName =
            Path.GetFileName(file.FileName);

        var uniqueName =
            $"{Guid.NewGuid()}_{safeFileName}";

        var path = Path.Combine(
            uploadsFolder,
            uniqueName);

        await using var stream =
            new FileStream(path, FileMode.Create);

        await file.CopyToAsync(stream);

        sensor.AttachmentFileName = uniqueName;

        return Ok(new
        {
            Message = "File uploaded successfully.",
            FileName = uniqueName,
            DeviceId = deviceId
        });
    }
}
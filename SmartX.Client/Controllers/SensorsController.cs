using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using SmartX.Client.Models;

namespace SmartX.Client.Controllers;

public class SensorsController : Controller
{
    private readonly IHttpClientFactory _factory;

    public SensorsController(
        IHttpClientFactory factory)
    {
        _factory = factory;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(
        SensorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var client =
            _factory.CreateClient("SmartXApi");

        var response =
            await client.PostAsJsonAsync(
                "api/sensors/register",
                model);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] =
                "Sensor registered successfully.";

            return RedirectToAction("Index",
                "Dashboard");
        }

        ModelState.AddModelError(
            "",
            "Sensor registration failed.");

        return View(model);
    }
}
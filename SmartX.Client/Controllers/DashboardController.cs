using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SmartX.Client.Models;

namespace SmartX.Client.Controllers;

public class DashboardController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DashboardController(
        IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var client =
            _httpClientFactory.CreateClient("SmartXApi");

        var response =
            await client.GetAsync("api/dashboard");

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Error =
                "Unable to connect to Smart-X API.";

            return View(new DashboardViewModel());
        }

        var json =
            await response.Content.ReadAsStringAsync();

        var dashboard =
            JsonSerializer.Deserialize<DashboardViewModel>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        return View(
            dashboard ?? new DashboardViewModel());
    }
}
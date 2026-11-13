using Microsoft.AspNetCore.Mvc;
namespace SmartX.Client.Controllers;
public sealed class NetworkController : Controller
{
    public IActionResult Index() => View();
}

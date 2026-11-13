using Microsoft.AspNetCore.Mvc;
namespace SmartX.Client.Controllers;
public sealed class CommandsController : Controller
{
    public IActionResult Index() => View();
}

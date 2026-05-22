using Microsoft.AspNetCore.Mvc;

namespace Restaurant.Web.Controllers;

public class WaiterController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}
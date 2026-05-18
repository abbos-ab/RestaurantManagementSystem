using Microsoft.AspNetCore.Mvc;

namespace Restaurant.Web.Controllers;

public class TableController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}
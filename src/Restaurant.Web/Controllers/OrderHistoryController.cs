using Microsoft.AspNetCore.Mvc;

namespace Restaurant.Web.Controllers;

public class OrderHistoryController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}
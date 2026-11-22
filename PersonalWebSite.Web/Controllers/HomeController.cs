using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PersonalWebSite.Web.Models;

namespace PersonalWebSite.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    // Hem işlenmeyen hatalar (/Home/Error) hem de 404 gibi durum kodları (/Home/Error/404) buraya düşer.
    [Route("Home/Error/{code?}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? code)
    {
        var statusCode = code ?? 500;
        Response.StatusCode = statusCode;

        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            StatusCode = statusCode
        });
    }
}

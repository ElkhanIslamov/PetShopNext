using Microsoft.AspNetCore.Mvc;

namespace Pb304PetShop.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Title = "View bag title";
            ViewData["Title1"] = "View data title";
            TempData["Titlet"] = "Temp data title";
            
            return View();
        }

        public IActionResult Temp()
        {
            return View();
        }
    }

    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pb304PetShop.DataContext;
using Pb304PetShop.Models;

namespace Pb304PetShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var sliders = await _dbContext.Sliders.ToListAsync();
            var products = await _dbContext.Products.Take(6).ToListAsync();
            var categories = await _dbContext.Categories.ToListAsync();

            var model = new HomeViewModel
            {
                Sliders = sliders,
                Products = products,
                Categories = categories
            };

            return View(model);
        }

        public IActionResult Test()
        {
            Response.Cookies.Append("Test", "Pb304-New",new CookieOptions { Expires = DateTimeOffset.Now.AddHours(1)});

            HttpContext.Session.SetString("Test-Session", "Pb304-Session");

            return NoContent();
        }

        public IActionResult Get()
        {
            var test = Request.Cookies["Test"];
            var testSession = HttpContext.Session.GetString("Test-Session");
            return Json(new { test, testSession });
        }
    }
}

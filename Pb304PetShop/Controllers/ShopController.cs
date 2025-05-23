using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pb304PetShop.DataContext;

namespace Pb304PetShop.Controllers
{
    [Authorize]
    public class ShopController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ShopController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ProductCount = await _dbContext.Products.CountAsync();

            var products = await _dbContext.Products.Take(6).ToListAsync();

            return View(products);
        }

        //[HttpPost]
        //public async Task<IActionResult> Partial([FromBody]RequestModel requestModel)
        //{
        //    var products = await _dbContext.Products.Skip(requestModel.StartFrom).Take(6).ToListAsync();

        //    return PartialView("_ProductPartialView", products);
        //}

        [HttpPost]
        public async Task<IActionResult> Partial([FromBody] RequestModel requestModel)
        {
            var products = await _dbContext.Products.Skip(requestModel.StartFrom).Take(6).ToListAsync();

            return Json(products);
        }

    }

    public class RequestModel
    {
        public int StartFrom { get; set; }
    }
}

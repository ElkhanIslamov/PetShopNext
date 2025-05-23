using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pb304PetShop.DataContext;
using Pb304PetShop.Models;

namespace Pb304PetShop.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _dbContext;
        private const string BasketCookieKey = "Basket";

        public BasketController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddToBasket(int id)
        {
            var product = _dbContext.Products.Find(id);

            if (product == null)
            {
                return BadRequest();
            }

            var basket = GetBasket();

            var exitBasketItem = basket.Find(x => x.ProductId == id);

            if (exitBasketItem == null)
            {
                basket.Add(new BasketItemCookieModel { ProductId = id, Quantity = 1 });

            }
            else
            {
                exitBasketItem.Quantity++;
            }

            var basketJson = JsonConvert.SerializeObject(basket);

            Response.Cookies.Append(BasketCookieKey, basketJson, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddHours(1)
            });

            return RedirectToAction("Index", "Home");
        }

        private List<BasketItemCookieModel> GetBasket()
        {
            var basket = Request.Cookies[BasketCookieKey];

            if (string.IsNullOrEmpty(basket))
            {
                return new List<BasketItemCookieModel>();
            }

            var basketItems = JsonConvert.DeserializeObject<List<BasketItemCookieModel>>(basket);

            if (basketItems == null)
            {
                return new List<BasketItemCookieModel>();
            }

            return basketItems;
        }

        public async Task<IActionResult> Cart()
        {
            var basket = Request.Cookies["Basket"];
            if (string.IsNullOrEmpty(basket))
            {
                return Content("0");
            }

            var basketItems = JsonConvert.DeserializeObject<List<BasketItemCookieModel>>(basket);

            var cart = new CartViewModel();
            var cartItemList = new List<CartItemViewModel>();

            foreach (var item in basketItems ?? [])
            {
                var product = await _dbContext.Products.FindAsync(item.ProductId);

                if (product == null) continue;

                cartItemList.Add(new CartItemViewModel
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Description = product.Name,
                    Price = product.Price,
                    Quantity = item.Quantity,
                    ImageUrl = product.CoverImageUrl
                });
            }

            cart.Items = cartItemList;
            cart.Quantity = cartItemList.Sum(x => x.Quantity);
            cart.Total = cartItemList.Sum(x => x.Quantity * x.Price);

            return View(cart);
        }

        [HttpPost]
        public IActionResult UpdateBasket([FromBody]UpdateModel updateModel)
        {
            var basket = GetBasket();
            var basketItem = basket.Find(x => x.ProductId == updateModel.ProductId);
            if (basketItem != null)
            {
                if (updateModel.Quantity <= 0)
                {
                    basket.Remove(basketItem);
                }
                else
                {
                    basketItem.Quantity = updateModel.Quantity;
                }
            }
            var basketJson = JsonConvert.SerializeObject(basket);
            Response.Cookies.Append(BasketCookieKey, basketJson, new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddMonths(1)
            });

            var cart = new CartViewModel();
            var cartItemList = new List<CartItemViewModel>();

            foreach (var item in basket ?? [])
            {
                var product = _dbContext.Products.Find(item.ProductId);

                if (product == null) continue;

                cartItemList.Add(new CartItemViewModel
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Description = product.Name,
                    Price = product.Price,
                    Quantity = item.Quantity,
                    ImageUrl = product.CoverImageUrl
                });
            }

            cart.Items = cartItemList;
            cart.Quantity = cartItemList.Sum(x => x.Quantity);
            cart.Total = cartItemList.Sum(x => x.Quantity * x.Price);

            return PartialView("_CartPartialView", cart);
        }
    }

    public class UpdateModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}

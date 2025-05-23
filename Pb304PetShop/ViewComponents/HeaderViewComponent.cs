using Microsoft.AspNetCore.Mvc;
using Pb304PetShop.Models;

namespace Pb304PetShop.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var basket = Request.Cookies["Basket"];
            var quantity = 0;

            if (!string.IsNullOrEmpty(basket))
            {
                var basketItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BasketItemCookieModel>>(basket) ?? [];
                quantity = basketItems.Sum(x => x.Quantity);
            }

            var headerViewModel = new HeaderViewModel
            {
                Quantity = quantity
            };

            return View(headerViewModel);
        }
    }
}

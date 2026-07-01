using Microsoft.AspNetCore.Mvc;
using WebAccGameMVC1.Services;

namespace WebAccGameMVC1.ViewComponents
{
    public class CartBadgeViewComponent : ViewComponent
    {
        private readonly CartService _cartService;

        public CartBadgeViewComponent(CartService cartService)
        {
            _cartService = cartService;
        }

        public IViewComponentResult Invoke()
        {
            var count = _cartService.GetCart().Items.Sum(i => i.Quantity);
            return View("Default", count);
        }
    }
}

using BistroBlaze_API.Data;
using BistroBlaze_API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BistroBlaze_API.Controllers
{
    [Route("api/shoppingcart")]
    [ApiController]
    public class ShoppingCartController : Controller
    {
        protected ApiResponse _response;
        private readonly ApplicationDbContext _db;
        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
            _response = new ApiResponse();
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> AddOrUpdateItemsInCart(string userId, int menuItemId, int updateQuantityBy)
        {
            //Shopping cart has 1 entry per user id, even if user has many items in the cart
            //Cart items will have all items in shopping cart for user


            // cases 
            // when a user adds a new item to a new shopping cart for the first time
            // when user adds a new item to existing shopping cart  (has other items in cart)
            // when user updates existing item count
            // when user removes existing item
            ShoppingCart shoppingCart = _db.ShoppingCarts.FirstOrDefault(u => u.UserId.Equals(userId));
            MenuItem menuItem = _db.MenuItems.FirstOrDefault(u => u.Id.Equals(menuItemId));
            if(menuItem == null) 
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            if(shoppingCart == null && updateQuantityBy > 0)
            {
                //create shopping cart, add cart item
                ShoppingCart newCart = new()
                {
                    UserId = userId
                };
                _db.ShoppingCarts.Add(newCart);
                await _db.SaveChangesAsync();

                CartItem newCartItem = new() 
                {   MenuItemId = menuItemId,
                    Quantity = updateQuantityBy,
                    ShoppingCartId = newCart.Id,
                    MenuItem = null
                };
                _db.CartItems.Add(newCartItem);
                await _db.SaveChangesAsync();

            }
            else
            {
                //shopping cart exists

                CartItem cartItemInDb = _db.CartItems.FirstOrDefault(u => u.MenuItemId == menuItemId);

            }
        
        }
    }
}

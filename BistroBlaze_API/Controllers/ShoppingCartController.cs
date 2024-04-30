using BistroBlaze_API.Data;
using BistroBlaze_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetShoppingCart(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                ShoppingCart shoppingCart = _db.ShoppingCarts
                            .Include(u => u.CartItems)
                            .ThenInclude(u => u.MenuItem)
                            .FirstOrDefault(u => u.UserId == userId);
                if(shoppingCart.CartItems != null && shoppingCart.CartItems.Count() > 0)
                {
                    shoppingCart.CartTotal = shoppingCart.CartItems.Sum(u => u.Quantity*u.MenuItem.Price);
                }
                _response.Result = shoppingCart;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                _response.StatusCode = HttpStatusCode.BadRequest;
            }
            return _response;


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
            ShoppingCart shoppingCart = _db.ShoppingCarts.Include(u => u.CartItems).FirstOrDefault(u => u.UserId.Equals(userId));
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

                CartItem cartItemInCart = _db.CartItems.FirstOrDefault(u => u.MenuItemId == menuItemId);
                //item doesn't exist in shopping cart
                if(cartItemInCart == null) {

                    CartItem newCartItem = new()
                    {
                        MenuItemId = menuItemId,
                        Quantity = updateQuantityBy,
                        ShoppingCartId = shoppingCart.Id,
                        MenuItem = null
                    };
                    _db.CartItems.Add(newCartItem);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //item exists, update quantity
                    int newQuantity = cartItemInCart.Quantity + updateQuantityBy;
                    if(updateQuantityBy <= 0 || newQuantity <= 0)
                    {
                        //remove cart item from cart
                        _db.CartItems.Remove(cartItemInCart);
                        if(shoppingCart.CartItems.Count() == 1)
                        {
                            _db.ShoppingCarts.Remove(shoppingCart);
                        }
                        _db.SaveChanges();
                    }
                    else
                    {
                        cartItemInCart.Quantity = cartItemInCart.Quantity = newQuantity;
                        _db.SaveChanges();
                    }
                }
                
            }
            return _response;
        
        }
    }
}

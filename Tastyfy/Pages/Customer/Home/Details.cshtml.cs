using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Security.Claims;
using Tastyfy.DataAccess.Data.Repository.IRepository;
using Tastyfy.Models;
using Tastyfy.Utility;

namespace Tastyfy.Pages.Customer.Home
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public DetailsModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public ShoppingCart ShoppingCartObj { get; set; }

        public void OnGet(int id)
        {
            ShoppingCartObj = new ShoppingCart()
            {
                MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(includeProperties: "Category,FoodType", filter: c => c.Id == id),
                MenuItemId = id
            };
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                //Gets the claim of the user logged in
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                ShoppingCartObj.ApplicationUserId = claims.Value;

                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(c =>
                    c.ApplicationUserId == ShoppingCartObj.ApplicationUserId &&
                    c.MenuItemId == ShoppingCartObj.MenuItemId);

                if (cartFromDb == null)
                {
                    //user has no cart in db
                    _unitOfWork.ShoppingCart.Add(ShoppingCartObj);
                }
                else
                {
                    _unitOfWork.ShoppingCart.IncrementQuantity(cartFromDb, ShoppingCartObj.Count);
                }
                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == ShoppingCartObj.ApplicationUserId)
                    .ToList()
                    .Count;

                HttpContext.Session.SetInt32(SD.ShoppingCart, count);

                return RedirectToPage("Index");
            }
            else
            {
                ShoppingCartObj.MenuItem =
                    _unitOfWork.MenuItem.GetFirstOrDefault(includeProperties: "Category,FoodType",
                        filter: c => c.Id == ShoppingCartObj.MenuItemId);
                return Page();
            }
        }
    }
}
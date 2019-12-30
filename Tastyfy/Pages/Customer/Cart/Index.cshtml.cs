using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Tastyfy.DataAccess.Data.Repository.IRepository;
using Tastyfy.Models;
using Tastyfy.Models.ViewModels;
using Tastyfy.Utility;

namespace Tastyfy.Pages.Customer.Cart
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public OrderDetailsCartViewModel OrderDetailsCartViewModel { get; set; }

        public void OnGet()
        {
            OrderDetailsCartViewModel = new OrderDetailsCartViewModel()
            {
                OrderHeader = new OrderHeader()
                {
                    OrderTotal = 0
                },
                ListCart = new List<ShoppingCart>()
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                IEnumerable<ShoppingCart> cart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value);

                if (cart != null)
                {
                    OrderDetailsCartViewModel.ListCart = cart.ToList();
                }

                foreach (var shoppingCart in OrderDetailsCartViewModel.ListCart)
                {
                    shoppingCart.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(m => m.Id == shoppingCart.MenuItemId);
                    OrderDetailsCartViewModel.OrderHeader.OrderTotal += (shoppingCart.MenuItem.Price * shoppingCart.Count);
                }
            }
        }

        public IActionResult OnPostPlus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementQuantity(cart, 1);
            _unitOfWork.Save();

            return RedirectToPage("/Customer/Cart/Index");
        }

        public IActionResult OnPostMinus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);

            if (cart.Count == 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
                _unitOfWork.Save();

                UpdateCountForSession(cart);
            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementQuantity(cart, 1);
                _unitOfWork.Save();
            }

            return RedirectToPage("/Customer/Cart/Index");
        }

        public IActionResult OnPostRemove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);

            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();

            UpdateCountForSession(cart);

            return RedirectToPage("/Customer/Cart/Index");
        }

        public void UpdateCountForSession(ShoppingCart cart)
        {
            var cnt = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == cart.ApplicationUserId)
                .ToList()
                .Count;

            HttpContext.Session.SetInt32(SD.ShoppingCart, cnt);
        }
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Tastyfy.DataAccess.Data.Repository.IRepository;
using Tastyfy.Models;
using Tastyfy.Models.ViewModels;
using Tastyfy.Utility;

namespace Tastyfy.Pages.Customer.Cart
{
    public class SummaryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public SummaryModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public OrderDetailsCartViewModel DetailsCart { get; set; }

        public IActionResult OnGet()
        {
            DetailsCart = new OrderDetailsCartViewModel()
            {
                OrderHeader = new OrderHeader()
                {
                    OrderTotal = 0
                }
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<ShoppingCart> cart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                DetailsCart.ListCart = cart.ToList();
            }

            foreach (var shoppingCart in DetailsCart.ListCart)
            {
                shoppingCart.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(m => m.Id == shoppingCart.MenuItemId);
                DetailsCart.OrderHeader.OrderTotal += (shoppingCart.MenuItem.Price * shoppingCart.Count);
            }

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(c => c.Id == claim.Value);
            DetailsCart.OrderHeader.PickUpName = applicationUser.FullName;
            DetailsCart.OrderHeader.PickupTime = DateTime.Now;
            DetailsCart.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;

            return Page();
        }

        public IActionResult OnPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //Gets the list cart from the database
            DetailsCart.ListCart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value).ToList();

            //Sets the OrderHear
            DetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            DetailsCart.OrderHeader.OrderDate = DateTime.Now;
            DetailsCart.OrderHeader.UserId = claim.Value;
            DetailsCart.OrderHeader.Status = SD.PaymentStatusPending;

            //Pickup time will be the pickup date + pickup time
            DetailsCart.OrderHeader.PickupTime = Convert.ToDateTime(DetailsCart.OrderHeader.PickUpDate.ToShortDateString() + " " + DetailsCart.OrderHeader.PickupTime.ToShortTimeString());

            //Create new OrderDetails to add in db
            List<OrderDetails> orderDetailsList = new List<OrderDetails>();

            //Add the OrderHeader to db
            _unitOfWork.OrderHeader.Add(DetailsCart.OrderHeader);
            _unitOfWork.Save();

            //Looping each item in the cart and adding it to order details to adding it to the database
            foreach (var item in DetailsCart.ListCart)
            {
                item.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(m => m.Id == item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails()
                {
                    MenuItemId = item.MenuItemId,
                    Name = item.MenuItem.Name,
                    OrderId = DetailsCart.OrderHeader.Id,
                    Description = item.MenuItem.Description,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                DetailsCart.OrderHeader.OrderTotal += (orderDetails.Count * orderDetails.Price);
                _unitOfWork.OrderDetails.Add(orderDetails);
            }

            DetailsCart.OrderHeader.OrderTotal = Convert.ToDouble($"{DetailsCart.OrderHeader.OrderTotal:.##}");

            //Removes all the item from the shopping cart in the database through the remove range from repository
            _unitOfWork.ShoppingCart.RemoveRange(DetailsCart.ListCart);
            //Clear the session context so the user sees 0 items
            HttpContext.Session.SetInt32(SD.ShoppingCart, 0);

            //Do this here for efficiency
            _unitOfWork.Save();

            //Setting the payment for stripe
            if (stripeToken != null)
            {
                //Charging a card for stripe
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(DetailsCart.OrderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Description = "Order Id: " + DetailsCart.OrderHeader.Id,
                    Source = stripeToken
                };

                var service = new ChargeService();
                Charge charge = service.Create(options);

                DetailsCart.OrderHeader.TransactionId = charge.Id;

                //checking how the payment went
                if (charge.Status.ToLower() == "succeeded")
                {
                    //email
                    DetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    DetailsCart.OrderHeader.Status = SD.StatusSubmitted;
                }
                else
                {
                    DetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                }
            }
            else
            {
                DetailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }

            _unitOfWork.Save();

            return RedirectToPage("/Customer/Cart/OrderConfirmation", new { id = DetailsCart.OrderHeader.Id });
        }
    }
}
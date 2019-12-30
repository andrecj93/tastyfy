using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Tastyfy.DataAccess.Data.Repository.IRepository;
using Tastyfy.Models;
using Tastyfy.Models.ViewModels;
using Tastyfy.Utility;

namespace Tastyfy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get(string status = null)
        {
            List<OrderDetailsViewModel> orderListVm = new List<OrderDetailsViewModel>();

            IEnumerable<OrderHeader> orderHeadersList;

            if (User.IsInRole(SD.CustomerRole))
            {
                //RETRIEVE ALL ORDER FOR THAT CUSTOMER
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                orderHeadersList =
                    _unitOfWork.OrderHeader.GetAll(u => u.UserId == claim.Value, null, "ApplicationUser");
            }
            else
            {
                orderHeadersList =
                    _unitOfWork.OrderHeader.GetAll(null, null, "ApplicationUser");
            }

            if (status == SD.StatusCanceled)
            {
                orderHeadersList =
                    orderHeadersList.Where(o => o.Status == SD.StatusCanceled || o.Status == SD.StatusRefunded || o.Status == SD.PaymentStatusRejected);
            }
            else
            {
                if (status == SD.StatusCompleted)
                {
                    orderHeadersList =
                        orderHeadersList.Where(o => o.Status == SD.StatusCompleted);
                }
                else
                {
                    orderHeadersList =
                        orderHeadersList.Where(o => o.Status == SD.StatusReady || o.Status == SD.StatusInProcess || o.Status == SD.StatusSubmitted || o.Status == SD.PaymentStatusPending);
                }
            }

            foreach (var orderHeader in orderHeadersList)
            {
                OrderDetailsViewModel model = new OrderDetailsViewModel()
                {
                    OrderHeader = orderHeader,
                    OrderDetailsList = _unitOfWork.OrderDetails.GetAll(c => c.OrderId == orderHeader.Id).ToList()
                };
                orderListVm.Add(model);
            }

            return Json(new { data = orderListVm });
        }
    }
}
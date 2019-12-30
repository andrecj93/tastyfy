using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using Tastyfy.DataAccess.Data.Repository.IRepository;
using Tastyfy.Models;
using Tastyfy.Models.ViewModels;
using Tastyfy.Utility;

namespace Tastyfy.Pages.Admin.Order
{
    public class ManageOrderModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public ManageOrderModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public List<OrderDetailsViewModel> OrderDetailsViewModels { get; set; }

        public void OnGet()
        {
            OrderDetailsViewModels = new List<OrderDetailsViewModel>();

            List<OrderHeader> orderHeadersList = _unitOfWork.OrderHeader
                .GetAll(o => o.Status == SD.StatusSubmitted || o.Status == SD.StatusInProcess)
                .OrderByDescending(u => u.PickupTime)
                .ToList();

            foreach (var orderHeader in orderHeadersList)
            {
                OrderDetailsViewModel model = new OrderDetailsViewModel()
                {
                    OrderHeader = orderHeader,
                    OrderDetailsList = _unitOfWork.OrderDetails.GetAll(c => c.OrderId == orderHeader.Id).ToList()
                };
                OrderDetailsViewModels.Add(model);
            }
        }

        public IActionResult OnPostOrderPrepare(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(c => c.Id == orderId);
            orderHeader.Status = SD.StatusInProcess;
            _unitOfWork.Save();
            return RedirectToPage("ManageOrder", new { inProcess = "true" });
        }

        public IActionResult OnPostOrderReady(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(c => c.Id == orderId);
            orderHeader.Status = SD.StatusReady;
            _unitOfWork.Save();
            return RedirectToPage("ManageOrder", new { orderReady = "true" });
        }

        public IActionResult OnPostOrderCancel(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(c => c.Id == orderId);
            orderHeader.Status = SD.StatusCanceled;
            _unitOfWork.Save();
            return RedirectToPage("ManageOrder", new { cancelled = "true" });
        }

        public IActionResult OnPostOrderRefund(int orderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(c => c.Id == orderId);

            //refund the amount

            var options = new RefundCreateOptions
            {
                Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                Charge = orderHeader.TransactionId,
                Reason = RefundReasons.RequestedByCustomer
            };
            var service = new RefundService();
            service.Create(options);

            orderHeader.Status = SD.StatusRefunded;
            _unitOfWork.Save();
            return RedirectToPage("ManageOrder", new { refunded = "true" });
        }
    }
}
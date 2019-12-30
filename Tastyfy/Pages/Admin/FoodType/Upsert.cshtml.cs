﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tastyfy.DataAccess.Data.Repository.IRepository;
using Tastyfy.Utility;

namespace Tastyfy.Pages.Admin.FoodType
{
    [Authorize(Roles = SD.ManagerRole)]
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpsertModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public Models.FoodType FoodTypeObj { get; set; }

        public IActionResult OnGet(int? id)
        {
            FoodTypeObj = new Models.FoodType();

            //If has id its a edit request
            if (id != null)
            {
                FoodTypeObj = _unitOfWork.FoodType.GetFirstOrDefault(c => c.Id == id);
                if (FoodTypeObj == null)
                {
                    return NotFound();
                }
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (FoodTypeObj.Id != 0)
            {
                _unitOfWork.FoodType.Update(FoodTypeObj);
            }
            else
            {
                _unitOfWork.FoodType.Add(FoodTypeObj);
            }

            _unitOfWork.Save();

            return RedirectToPage("./Index");
        }
    }
}
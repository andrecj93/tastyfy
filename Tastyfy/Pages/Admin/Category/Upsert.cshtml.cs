using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tastyfy.DataAccess.Data.Repository.IRepository;
using Tastyfy.Utility;

namespace Tastyfy.Pages.Admin.Category
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
        public Models.Category CategoryObj { get; set; }

        public IActionResult OnGet(int? id)
        {
            CategoryObj = new Models.Category();

            //If has id its a edit request
            if (id != null)
            {
                CategoryObj = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
                if (CategoryObj == null)
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

            if (CategoryObj.Id != 0)
            {
                _unitOfWork.Category.Update(CategoryObj);
            }
            else
            {
                _unitOfWork.Category.Add(CategoryObj);
            }

            _unitOfWork.Save();

            return RedirectToPage("./Index");
        }
    }
}
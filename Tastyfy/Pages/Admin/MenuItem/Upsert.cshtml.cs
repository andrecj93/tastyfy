using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;
using Tastyfy.DataAccess.Data.Repository.IRepository;
using Tastyfy.Models.ViewModels;
using Tastyfy.Utility;

namespace Tastyfy.Pages.Admin.MenuItem
{
    [Authorize(Roles = SD.ManagerRole)]
    public class UpsertModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UpsertModel(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment;
        }

        [BindProperty]
        public MenuItemViewModel MenuItemViewModel { get; set; }

        public IActionResult OnGet(int? id)
        {
            MenuItemViewModel = new MenuItemViewModel
            {
                CategoryList = _unitOfWork.Category.GetCategoryListForDropDown(),
                FoodTypeList = _unitOfWork.FoodType.GetFoodTypeListItems(),
                MenuItem = new Models.MenuItem()
            };

            //If has id its a edit request
            if (id != null)
            {
                MenuItemViewModel.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(c => c.Id == id);
                if (MenuItemViewModel.MenuItem == null)
                {
                    return NotFound();
                }
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            bool hasImage = false;
            string fileName = "";
            var uploads = Path.Combine(webRootPath, @"images\menuItems");
            string extension = "";

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (files.Count > 0)
            {
                hasImage = true;
                fileName = Guid.NewGuid().ToString();
                extension = Path.GetExtension(files[0].FileName);
            }

            //Create Post Action
            if (MenuItemViewModel.MenuItem.Id == 0)
            {
                if (hasImage)
                {
                    using (var filestream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }

                    MenuItemViewModel.MenuItem.Image = @"\images\menuItems\" + fileName + extension;
                }

                _unitOfWork.MenuItem.Add(MenuItemViewModel.MenuItem);
            }
            else
            {
                //Update Post Action
                var objFromDb = _unitOfWork.MenuItem.Get(MenuItemViewModel.MenuItem.Id);

                if (hasImage)
                {
                    //Remove existing image and update to a new one
                    string imageLocalExists = Path.Combine(webRootPath, objFromDb.Image.TrimStart('\\'));

                    if (System.IO.File.Exists(imageLocalExists))
                    {
                        System.IO.File.Delete(imageLocalExists);
                    }

                    using (var filestream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }
                }
                else
                {
                    MenuItemViewModel.MenuItem.Image = objFromDb.Image;
                }

                _unitOfWork.MenuItem.Udpate(MenuItemViewModel.MenuItem);
            }

            _unitOfWork.Save();

            return RedirectToPage("./Index");
        }
    }
}
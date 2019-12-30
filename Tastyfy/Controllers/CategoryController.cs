using Microsoft.AspNetCore.Mvc;
using Tastyfy.DataAccess.Data.Repository.IRepository;
using Tastyfy.Models;

namespace Tastyfy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Get()
        {
            //return Json(new { data = _unitOfWork.SpCall.ReturnList<Category>("usp_GetAllCategory", null) });
            return Json(new {data = _unitOfWork.Category.GetAll()});
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);

            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Category.Remove(objFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });
        }
    }
}
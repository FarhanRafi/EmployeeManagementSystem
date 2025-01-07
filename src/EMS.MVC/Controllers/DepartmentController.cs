using Microsoft.AspNetCore.Mvc;

namespace EMS.MVC.Controllers
{
    public class DepartmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

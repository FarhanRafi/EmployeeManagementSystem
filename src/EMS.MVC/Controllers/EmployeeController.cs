using Microsoft.AspNetCore.Mvc;

namespace EMS.MVC.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

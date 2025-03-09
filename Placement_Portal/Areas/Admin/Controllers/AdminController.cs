using Microsoft.AspNetCore.Mvc;

namespace Placement_Portal.Areas.Admin.Controllers
{
    [CheckAccess]

    [Area("Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

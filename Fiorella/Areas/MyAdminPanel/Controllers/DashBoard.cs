using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiorella.Areas.MyAdminPanel.Controllers
{
    [Area("MyAdminPanel")]
    [Authorize]
    public class DashBoard : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Areas.BoardManagement.Controllers
{
    public class BoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ReportManager()
        {
            return View();
        }

        public IActionResult ArticleManager()
        {
            return View();
        }

    }
}

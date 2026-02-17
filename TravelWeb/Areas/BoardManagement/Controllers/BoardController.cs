using Microsoft.AspNetCore.Mvc;
using TravelWeb.Areas.BoardManagement.Models.BoardDB;

namespace TravelWeb.Areas.BoardManagement.Controllers
{
    [Area("BoardManagement")]
    public class BoardController : Controller
    {
        private readonly BoardDbContext _DbContext;
        public BoardController(BoardDbContext DbContext)
        {
            _DbContext = DbContext;
        }



        public IActionResult Index()
        {
            return View(); 
        }

        public IActionResult ReportManager()
        {
            var ModerationLog = _DbContext.ViewArticleHideStatuses.ToList();
            return View(ModerationLog);
           
        }

        public IActionResult ArticleManager()
        {
            return View();
        }

        public IActionResult ReportDetails(int ID)
        {
            IEnumerable<ReportLog> reportLog = _DbContext.ReportLogs.Where(r => r.TargetId==ID).ToList();
            return View(reportLog);
        }

        public IActionResult CommentManager()
        {
           var articles = _DbContext.Articles.ToList();
            return View(articles);
            //return View();
        }

    }
}

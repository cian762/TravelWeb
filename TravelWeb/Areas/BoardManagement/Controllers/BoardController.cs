using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.BoardManagement.Models.BoardDB;
using TravelWeb.Areas.BoardManagement.Models.IService;
using TravelWeb.Areas.BoardManagement.Models.Service;
using TravelWeb.Areas.BoardManagement.Models.ViewModel;
using TravelWeb.Models;
using static TravelWeb.Areas.BoardManagement.Models.ViewModel.PostCardModel;

namespace TravelWeb.Areas.BoardManagement.Controllers
{
    [Area("BoardManagement")]
    public class BoardController : Controller
    {
        private readonly BoardDbContext _DbContext;
        private readonly MemberSystemContext _memberDb;
        private readonly INoteService _noteService;
        public BoardController(BoardDbContext DbContext,
            INoteService noteService,
            MemberSystemContext memberDb)
        {
            _DbContext = DbContext;
            _noteService = noteService;
            _memberDb = memberDb;

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
            var viewModel = new ReportDetailsModel
            {
                LogItems = _noteService.GetReportLogs(ID),
                auditNote = _noteService.GetNote(ID)
            };            
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult UpdateNote(int id, string noteContent)
        {
            try
            {
                _noteService.UpdateNote(id, noteContent);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }


        [HttpPost]
        public IActionResult AddReport(int ArticleID,
                                       string UserId, 
                                       int ViolationType,
                                       string? ReportDetails)
        {
            try
            {
                var log = new ReportLog
                {
                    TargetId = ArticleID,
                    UserId = UserId,
                    ViolationType = (byte)(ViolationType - 1),                    
                    Status = 0,
                    Reason = ReportDetails

                };
                _noteService.AddReport(log);
                Console.WriteLine("AddReport");
                return Json(new { success = true});

            }
            catch(Exception ex)
            {
                // 1. 在伺服器後端印出錯誤原因
                Console.WriteLine($"AddReport Error: {ex.Message}");
                return Json(new { success = false });
            }
            finally
            {

            }
            

        }

        [HttpPost]
        public IActionResult UpdateReport(int LogID, int ResultID)
        {
            try
            { 
                Console.WriteLine($"{LogID} {ResultID}");
                _noteService.UpdateReportLog(LogID, ResultID);                
                return Json(new { success = true ,
                    updatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    
                });

            }
            catch
            {
                return Json(new { success = false });
            }
            finally
            {
                
            }


        }



        public IActionResult CommentManager()
        {
           var articles = _DbContext.Articles.ToList();
            return View(articles);
            //return View();
        }


        public IActionResult TestPost()
        {
            var articles = _DbContext.Articles.ToList();
            var members = _memberDb.MemberInformations.ToList();

            var viewModel = 

            // 使用 LINQ 把兩邊的資料合在一起
            articles.Select(a => new PostCardModel
            {
                ArticleID = a.ArticleId,
                Title = a.Title ?? "無標題",
                ArticlePhoto = a.PhotoUrl,
                // 去 Member 清單找誰是這篇文章的作者
                AuthorName = members.FirstOrDefault(m => m.MemberId == a.UserId)?.Name ?? "未命名",
                AuthorAvatar = members.FirstOrDefault(m => m.MemberId == a.UserId)?.AvatarUrl
            }).ToList();

            return View(viewModel);
        } 

    }
}

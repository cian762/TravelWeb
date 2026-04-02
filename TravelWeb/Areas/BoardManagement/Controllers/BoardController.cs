using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.Attractions.Models;
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


        public IActionResult  TagManager()
        {
            var tags = _DbContext.TagsLists.ToList();
            var articleTag = _DbContext.ArticleTags
                .GroupBy(x => x.TagId)
                .Select(g => new { TagId = g.Key, ArticleCount = g.Count() })
                .ToList();
            
            var viewModel = tags
                .GroupJoin(articleTag,
                    t => t.TagId,
                    at => at.TagId,
                    (t, at) => new BoardManagement.Models.ViewModel.Tag
                    {
                        ID = t.TagId,
                        Name = t.TagName,
                        Icon = t.icon,
                        ArticleCount = at.FirstOrDefault()?.ArticleCount ?? 0
                    })
                .ToList();


            return View(viewModel);
        }


        [HttpPost]
        public IActionResult CreateTag([FromBody] TagsList dto)
        {
            var tag = new TagsList { icon = dto.icon, TagName = dto.TagName };
            _DbContext.TagsLists.Add(tag);
            _DbContext.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult EditTag([FromBody] TagsList dto)
        {
            var tag = _DbContext.TagsLists.FirstOrDefault(t=>t.TagId==dto.TagId);
            if (tag == null) return Json(new { success = false, message = "找不到標籤" });

            tag.icon = dto.icon;
            tag.TagName = dto.TagName;
            _DbContext.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult DeleteTag(int id)
        {
            var articleCount = _DbContext.ArticleTags.Count(x => x.TagId == id);
            if (articleCount > 0)
                return Json(new { success = false, message = "此標籤仍有文章，無法刪除" });

            var tag = _DbContext.TagsLists.FirstOrDefault(t => t.TagId == id);
            if (tag == null) return Json(new { success = false, message = "找不到標籤" });

            _DbContext.TagsLists.Remove(tag);
            _DbContext.SaveChanges();
            return Json(new { success = true });
        }

    }
}

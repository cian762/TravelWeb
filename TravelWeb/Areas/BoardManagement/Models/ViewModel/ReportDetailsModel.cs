using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using TravelWeb.Areas.BoardManagement.Data;
using TravelWeb.Areas.BoardManagement.Models.BoardDB;

namespace TravelWeb.Areas.BoardManagement.Models.ViewModel
{
    public class ReportDetailsModel
    {
        public List<LogItem> LogItems { get; set; }
        public AuditNote auditNote { get; set; }


        public class LogItem
        {
            public int LogId { get; set; }
            public string UserId { get; set; }
            public string ViolationType { get; set; }
            public byte ResultType { get; set; }
            public string? Snapshot { get; set; }
            public string? Status { get; set; }
            public string? Photo { get; set; }
            public string? Reason { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public DateTime CreatedAt { get; set; } // 原始時間，留著備用
            // 在 ViewModel 裡準備好兩段文字
            public string CreateDateText => CreatedAt.ToString("yyyy/MM/dd");
            public string CreateTimeText => CreatedAt.ToString("tt hh:mm:ss");            

        }

        public IEnumerable<SelectListItem> ResultTypeLists =
                StaticConstants.ResultName.Select((name, index) => new SelectListItem
                {
                    Text = name,            // 顯示文字：判定無違規, 已通知修改...
                    Value = index.ToString() // 對應數值："0", "1", "2"...
                });


    }

}
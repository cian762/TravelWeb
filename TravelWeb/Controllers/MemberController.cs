using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class MemberController : Controller //會員資料管理
    {
        public IActionResult GetMemberList() //會員列表
        {
            return View();
        }

        public IActionResult GetMemberDetail()//會員詳細資料
        {
            return View();
        }

        public IActionResult EditMember() //編輯會員
        {
            return View();
        }

        public IActionResult SuspendMember() //停權會員
        {
            return View();
        }
    }
}

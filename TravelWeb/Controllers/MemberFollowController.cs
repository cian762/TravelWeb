using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class MemberFollowController : Controller //會員追蹤清單
    {
        public IActionResult GetFollowList() //查看被追蹤會員
        {
            return View();
        }

        public IActionResult AddFollow() //新增追蹤
        {
            return View();
        }

        public IActionResult RemoveFollow()//移除追蹤
        {
            return View();
        }
    }
}

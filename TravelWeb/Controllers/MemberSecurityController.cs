using Microsoft.AspNetCore.Mvc;

namespace TravelWeb.Controllers
{
    public class MemberSecurityController : Controller //資料加密 / 安全管理
    {
        public IActionResult EncryptMemberData() //密碼加密
        {
            return View();
        }

        public IActionResult DecryptMemberData() //個資保護
        {
            return View();
        }

        public IActionResult ResetPassword() //加解密處理
        {
            return View();
        }
    }
}

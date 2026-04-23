using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TravelWeb.Filters;

namespace TravelWeb.Filters // 確保命名空間與你的專案相符
{
    // 繼承 ActionFilterAttribute，讓它可以變成一個 [標籤]
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {   
            // 讀取目前的 Session 角色
            var role = context.HttpContext.Session.GetString("Role");

            // 如果沒有登入，或者角色不是 Admin，就攔截並導向登入頁！
            if (string.IsNullOrEmpty(role) || role != "Admin")
            {
                // 產生要跳轉的路由 (跳轉到 AuthController 的 Login Action)
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }

            // 如果驗證通過，就讓程式繼續往下執行原本的頁面
            base.OnActionExecuting(context);
        }
    }
}
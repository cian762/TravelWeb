using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Models;
using TravelWeb.ViewModels;

public class MemberProfileController : Controller
{
    private readonly MemberSystemContext _context;

    public MemberProfileController(MemberSystemContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string memberCode)
    {
        // 🔐 1️⃣ 檢查是否登入
        var currentUserCode = HttpContext.Session.GetString("UserCode");
        var role = HttpContext.Session.GetString("Role");

        if (string.IsNullOrEmpty(currentUserCode))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (string.IsNullOrEmpty(memberCode))
            return NotFound();

        var memberList = await _context.MemberLists
            .FirstOrDefaultAsync(m => m.MemberCode == memberCode);

        var memberInfo = await _context.MemberInformations
            .FirstOrDefaultAsync(m => m.MemberCode == memberCode);

        if (memberList == null || memberInfo == null)
            return NotFound();

        bool isOwner = currentUserCode == memberCode;
        bool isAdmin = role == "Admin";

        var viewModel = new MemberProfileViewModel
        {
            MemberId = memberInfo.MemberId,
            Name = memberInfo.Name,
            Status = memberInfo.Status,
            AvatarUrl = memberInfo.AvatarUrl,

            // 🔥 只有本人或管理員可以看到 Email / Phone
            Email = (isOwner || isAdmin) ? memberList.Email : null,
            Phone = (isOwner || isAdmin) ? memberList.Phone : null,

            IsOwner = isOwner,
            IsAdmin = isAdmin
        };

        var currentUserId = HttpContext.Session.GetString("UserCode");

        if (currentUserCode != null)
        {
            var currentUserInfo = await _context.MemberInformations
                .FirstOrDefaultAsync(m => m.MemberCode == currentUserCode);

            var targetUserInfo = await _context.MemberInformations
                .FirstOrDefaultAsync(m => m.MemberCode == memberCode);

            if (currentUserInfo != null && targetUserInfo != null)
            {
                bool isBlocked = await _context.Blockeds.AnyAsync(b =>
                    (b.MemberId == currentUserInfo.MemberId && b.BlockedId == targetUserInfo.MemberId) ||
                    (b.MemberId == targetUserInfo.MemberId && b.BlockedId == currentUserInfo.MemberId));

                if (isBlocked)
                {
                    return View("BlockedView");
                }
            }
        }

        return View(viewModel);
    }



    [HttpPost]
    public async Task<IActionResult> BlockUser(string blockedId, string reason)
    {
        var currentUserId = HttpContext.Session.GetString("UserCode");

        if (string.IsNullOrEmpty(currentUserId))
            return RedirectToAction("Login", "Auth");

        // 防止重複封鎖
        var alreadyBlocked = await _context.Blockeds.AnyAsync(b =>
            b.MemberId == currentUserId &&
            b.BlockedId == blockedId);

        if (alreadyBlocked)
            return RedirectToAction("Index", new { memberCode = blockedId });

        var block = new Blocked
        {
            MemberId = currentUserId,
            BlockedId = blockedId,
            BlockedDate = DateOnly.FromDateTime(DateTime.Now),
            Reason = reason
        };

        _context.Blockeds.Add(block);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", new { memberCode = blockedId });
    }


    [HttpPost]
    public async Task<IActionResult> Unblock(string blockedId)
    {
        var currentUserId = HttpContext.Session.GetString("UserCode");

        var block = await _context.Blockeds.FirstOrDefaultAsync(b =>
            b.MemberId == currentUserId &&
            b.BlockedId == blockedId);

        if (block != null)
        {
            _context.Blockeds.Remove(block);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", new { memberCode = blockedId });
    }


}


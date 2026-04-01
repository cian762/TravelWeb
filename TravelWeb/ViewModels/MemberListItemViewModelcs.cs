using System;

namespace TravelWeb.ViewModels
{
    // 1. 用於「詳細資料檢視」的 ViewModel
    public class MemberDetailsViewModel
    {
        public string MemberCode { get; set; }
        public string MemberId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }

        public string Name { get; set; }
        public string GenderText { get; set; } // 顯示 "男" 或 "女"
        public DateOnly? BirthDate { get; set; }
        public string AvatarUrl { get; set; }
        public string Status { get; set; }

        public DateTime? LastLoginTime { get; set; }
        public bool IsAlreadyAdmin { get; set; } // 用來判斷要不要隱藏「設為管理員」按鈕
    }

    // 2. 用於「編輯會員」的 ViewModel (刻意排除不可修改的欄位)
    public class MemberEditViewModel
    {
        public string MemberCode { get; set; } // 唯讀顯示用
        public string MemberId { get; set; }   // 唯讀顯示用
        public string Status { get; set; }     // 唯讀顯示用

        // 可編輯的欄位
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public byte? Gender { get; set; }
        public DateOnly? BirthDate { get; set; }
    }
}
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using TravelWeb.Areas.BoardManagement.Models.BoardDB;
using TravelWeb.Models;
namespace TravelWeb.Areas.BoardManagement.Models.ViewModel
{
    public class PostCardModel
    {

        public int ArticleID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ArticlePhoto { get; set; }       
        public string AuthorName { get; set; } = "未命名用戶";
        public string? AuthorAvatar { get; set; }

       // public List<PostItem> DisplayItems { get; set; } = new();
        
        //public required IEnumerable<Article> Articles { get; set; }        
        //public required IEnumerable<MemberInformation> Members { get; set; }
        
        //public string MemberName {
        //    get
        //    {
        //        return Members.FirstOrDefault()?.Name ?? "未命名用戶";
        //        //如果第一個問號問出來是null，執行後面??的結果
        //    }
        //} 
        //public string? MemberPhoto
        //{
        //    get
        //    {
        //        return Members.FirstOrDefault()?.AvatarUrl ?? string.Empty;
        //    }
        //}

        //public string ArticlePhoto
        //{
        //    get
        //    {
        //        return Articles.FirstOrDefault()?.PhotoUrl ?? string.Empty;
        //    }
        //}


        //public string ArticleTitle
        //{
        //    get
        //    {
        //        return Articles.FirstOrDefault()?.Title ?? "無標題文章";
        //    }
        //}

    }
    public class PostItem
    {
       
    }
}

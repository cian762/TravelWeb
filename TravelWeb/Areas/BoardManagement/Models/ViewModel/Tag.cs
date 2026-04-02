namespace TravelWeb.Areas.BoardManagement.Models.ViewModel
{
    public class Tag
    {

        public int ID { get; set; }
        public string Name { get; set; } = null!;
        public string? Icon { get; set; } = null!;
        
        public int ArticleCount { get; set; }   
    }
}

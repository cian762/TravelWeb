namespace TravelWeb.Areas.Itinerary.Models.ViewModel
{
    public class ComparisonAnalysisViewModel
    {
        // 總比較次數
        public int TotalComparisons { get; set; }

        // 每位會員使用比較功能次數
        public List<MemberComparisonStats> MemberStats { get; set; }

        // 每個行程被比較次數
        public List<ItineraryComparisonStats> ItineraryStats { get; set; }

        // 最近比較紀錄
        public List<RecentComparisonRecord> RecentComparisons { get; set; }
    }
    public class MemberComparisonStats
    {
        public string MemberID { get; set; }
        public string MemberName { get; set; }
        public int ComparisonCount { get; set; }
    }

    public class ItineraryComparisonStats
    {
        public int ItineraryID { get; set; }
        public string ItineraryName { get; set; }
        public int ComparisonCount { get; set; }
    }

    public class RecentComparisonRecord
    {
        public int ComparisonID { get; set; }
        public int OriginalVersionID { get; set; }
        public int ComparedVersionID { get; set; }
        public DateTime ComparedAt { get; set; }
    }
}


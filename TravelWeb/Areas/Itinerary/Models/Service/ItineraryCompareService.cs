using TravelWeb.Areas.Itinerary.Models.ItineraryDBModel;
using TravelWeb.Areas.Itinerary.Models.ViewModel;
using TravelWeb.Areas.Itinerary.Repository;

namespace TravelWeb.Areas.Itinerary.Models.Service
{
    public class ItineraryCompareService : IItineraryCompareService
    {
        private readonly IItineraryGenericRepository<ItineraryComparison> _itineraryCompare;

        private readonly IItineraryGenericRepository<ItineraryVersion> _itineraryVersion;
        private readonly IItineraryGenericRepository<ItineraryDBModel.Itinerary> _itinerary;
private readonly IItineraryGenericRepository<Member_Information>_memberInformation;
        public ItineraryCompareService(IItineraryGenericRepository<ItineraryComparison> com, IItineraryGenericRepository<ItineraryVersion>ver, IItineraryGenericRepository<ItineraryDBModel.Itinerary> it, IItineraryGenericRepository<Member_Information>mem)
        {
            _itineraryCompare = com;
            _itineraryVersion = ver;
            _itinerary = it;
            _memberInformation = mem;
        }
        public ComparisonAnalysisViewModel GetComparisonAnalysis()
        {
            // 1️⃣ 先取得資料
            var comparisons = _itineraryCompare.GetAll().ToList();
            var versions = _itineraryVersion.GetAll().ToList();
            var itineraries = _itinerary.GetAll().ToList();
            var members = _memberInformation.GetAll().ToList();

            var model = new ComparisonAnalysisViewModel();

            // 2️⃣ 總比較次數
            model.TotalComparisons = comparisons.Count;

            // 3️⃣ 會員使用統計
            model.MemberStats = (
                from c in comparisons
                join v in versions on c.OriginalVersionId equals v.VersionId
                join i in itineraries on v.ItineraryId equals i.ItineraryId
                join m in members on i.MemberId equals m.MemberId
                group c by new { m.MemberId, m.Name } into g
                select new MemberComparisonStats
                {
                    MemberID = g.Key.MemberId,
                    MemberName = g.Key.Name,
                    ComparisonCount = g.Count()
                }
            )
            .OrderByDescending(x => x.ComparisonCount)
            .ToList();

            // 4️⃣ 行程被比較次數
            model.ItineraryStats = (
                from c in comparisons
                join v in versions on c.OriginalVersionId equals v.VersionId
                join i in itineraries on v.ItineraryId equals i.ItineraryId
                group c by new { i.ItineraryId, i.ItineraryName } into g
                select new ItineraryComparisonStats
                {
                    ItineraryID = g.Key.ItineraryId,
                    ItineraryName = g.Key.ItineraryName,
                    ComparisonCount = g.Count()
                }
            )
            .OrderByDescending(x => x.ComparisonCount)
            .ToList();

            // 5️⃣ 最近比較紀錄（取10筆）
            model.RecentComparisons = comparisons
                .OrderByDescending(x => x.ComparisonTime)
                .Take(10)
                .Select(x => new RecentComparisonRecord
                {
                    ComparisonID = x.ComparisonId,
                    OriginalVersionID = x.OriginalVersionId,
                    ComparedVersionID = x.ComparedVersionId,
                    ComparedAt = x.ComparisonTime.Value
                })
                .ToList();

            return model;
        }
    }
    }

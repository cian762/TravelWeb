using TravelWeb.Areas.Itinerary.Models.ItineraryDBModel;
using TravelWeb.Areas.Itinerary.Models.ViewModel;
using TravelWeb.Areas.Itinerary.Repository;
using Microsoft.EntityFrameworkCore;

namespace TravelWeb.Areas.Itinerary.Models.Service
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IItineraryGenericRepository<ItineraryDBModel.Itinerary> _Itinerary;
        private readonly IItineraryGenericRepository<ItineraryVersion> _Version;
        private readonly IItineraryGenericRepository<Aianalysis> _Aianalysis;
        private readonly IItineraryGenericRepository<AigenerationError> _AigenerationError;
        public DashBoardService(IItineraryGenericRepository<ItineraryDBModel.Itinerary> itinerary, IItineraryGenericRepository<ItineraryVersion> verson, IItineraryGenericRepository<Aianalysis> aianalysis, IItineraryGenericRepository<AigenerationError> aierror)
        {
            _Aianalysis = aianalysis;
            _Version = verson;
            _Itinerary = itinerary;
            _AigenerationError = aierror;
        }
        public async Task<DashBoardViewModel> GetDashboardData()
        {
            var today = DateTime.Today;

            // 使用 await 並直接執行資料庫查詢，拿到結果
            var totalItineraries = await _Itinerary.GetAll().CountAsync();

            var todayItineraries = await _Itinerary.GetAll()
                .CountAsync(x => x.CreateTime.HasValue && x.CreateTime.Value.Date == today);

            var totalVersions = await _Version.GetAll().CountAsync();

            var todayAIGenerations = await _Aianalysis.GetAll()
                .CountAsync(x => x.AnalysisTime == today);

            var avgFeasibility = await _Aianalysis.GetAll()
                .AverageAsync(x => (double?)x.FeasibilityScore) ?? 0;

            var unconfirmedErrors = await _AigenerationError.GetAll()
                .CountAsync(x => !x.IsConfirmed);

            // --- 關鍵修正區：分開查詢 ---

            // 1. 先從資料庫取出 Version (限制 10 筆並轉成 List 釋放連線)
            var versions = await _Version.GetAll()
                .OrderByDescending(x => x.CreateTime)
                .Take(10)
                .ToListAsync();

            // 2. 獲取對應的 ItineraryId 列表
            var itineraryIds = versions.Select(v => v.ItineraryId).ToList();

            // 3. 從資料庫取出對應的 Itinerary (同樣轉成 List)
            var itineraries = await _Itinerary.GetAll()
                .Where(i => itineraryIds.Contains(i.ItineraryId))
                .ToListAsync();

            // 4. 在記憶體內進行 Join，這時候就不會動到資料庫連線了
            var recentActivities = versions.Join(itineraries,
                V => V.ItineraryId,
                I => I.ItineraryId,
                (V, I) => new ActivityViewModel
                {
                    Time = V.CreateTime ?? DateTime.Now,
                    UserID = I.MemberId
                }).ToList();

            return new DashBoardViewModel
            {
                TotalItineraries = totalItineraries,
                TodayItineraries = todayItineraries,
                TotalVersions = totalVersions,
                TodayAIGenerations = todayAIGenerations,
                AvgFeasibility = avgFeasibility,
                UnconfirmedErrors = unconfirmedErrors,
                RecentActivities = recentActivities
            };
    } }
}

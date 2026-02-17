using TravelWeb.Areas.Itinerary.Models.ItineraryDBModel;
using TravelWeb.Areas.Itinerary.Models.ViewModel;
using TravelWeb.Areas.Itinerary.Repository;

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
        public DashBoardViewModel GetDashboardData()
        {
            var today = DateTime.Today;

            var model = new DashBoardViewModel
            {
                TotalItineraries = _Itinerary.GetAll().Count(),

                TodayItineraries = _Itinerary
                    .GetAll()
                    .Count(x => x.CreateTime.Value.Date == today),

                TotalVersions = _Version.GetAll().Count(),

                TodayAIGenerations = _Aianalysis
                    .GetAll()
                    .Count(x => x.AnalysisTime == today),

                AvgFeasibility = _Aianalysis
                    .GetAll()
                    .Average(x => (double?)x.FeasibilityScore) ?? 0,

                UnconfirmedErrors = _AigenerationError
                    .GetAll()
                    .Count(x => !x.IsConfirmed),

                RecentActivities = _Version.GetAll().Join(_Itinerary.GetAll(), V => V.ItineraryId, I => I.ItineraryId, (V, I) => new ActivityViewModel
                {
                    Time = V.CreateTime.Value,
                    UserID = I.MemberId
                }).OrderByDescending(x => x.Time).Take(10).ToList() };

            return model;
        }
    }
}

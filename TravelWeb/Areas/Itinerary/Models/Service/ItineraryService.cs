using TravelWeb.Areas.Itinerary.Models.ItineraryDBModel;
using TravelWeb.Areas.Itinerary.Models.ViewModel;
using TravelWeb.Areas.Itinerary.Repository;

namespace TravelWeb.Areas.Itinerary.Models.Service
{
    public class ItineraryService:IItineraryService
    {
        private readonly IItineraryGenericRepository<ItineraryDBModel.Itinerary> _itineraryRepo;
        private readonly IItineraryGenericRepository<ItineraryVersion> _versionRepo;

        public ItineraryService(
            IItineraryGenericRepository<ItineraryDBModel.Itinerary> itineraryRepo,
            IItineraryGenericRepository<ItineraryVersion> versionRepo)
        {
            _itineraryRepo = itineraryRepo;
            _versionRepo = versionRepo;
        }

        public async Task<ItineraryDetailViewModel> GetItineraryManagementAsync()
        {
            var itineraries = _itineraryRepo.GetAll();

            var model = new ItineraryDetailViewModel
            {
                Itineraries = itineraries
                    .Select(i => new ItineraryItemVM
                    {
                        ItineraryId = i.ItineraryId,
                        Title = i.ItineraryName,
                        CurrentVersionNumber = i.ItineraryVersions.FirstOrDefault(v=>v.ItineraryId==i.ItineraryId).VersionNumber.ToString(),
                        CurrentUsageStatus = i.ItineraryVersions.FirstOrDefault(v => v.ItineraryId == i.ItineraryId).CurrentUsageStatus,
                        CreatedTime = i.ItineraryVersions.FirstOrDefault(v => v.ItineraryId == i.ItineraryId).CreateTime.Value,
                        VersionCount = i.ItineraryVersions.Count()
                    })  
                    .ToList()
            };

            return model;
        }

        public async Task<VersionViewModel> GetVersionManagementAsync(int itineraryId)
        {
            var itinerary = _itineraryRepo
                .GetAll()
                .FirstOrDefault(i => i.ItineraryId == itineraryId);

            var versions = _versionRepo
                .GetAll()
                .Where(v => v.ItineraryId == itineraryId)
                .OrderByDescending(v => v.VersionNumber);

            var model = new VersionViewModel
            {
                ItineraryId = itinerary.ItineraryId,
                ItineraryTitle = itinerary.ItineraryName,
                Versions = versions
                    .Select(v => new VersionItemVM
                    {
                        VersionId = v.VersionId,
                        VersionNumber = v.VersionNumber.Value,
                        Creator = v.Creator.ToString(),
                        CurrentUsageStatus = v.CurrentUsageStatus,
                        CreatedTime = v.CreateTime.Value
                    })
                    .ToList()
            };

            return model;
        }
    }
}

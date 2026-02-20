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

        public DiffViewModel GetDiff(int versionId)
        {
            var selectedVersion = _versionRepo.GetById(versionId);

            if (selectedVersion == null)
                throw new Exception("找不到版本");

            var currentVersion = _versionRepo.GetAll()
                .FirstOrDefault(v =>
                    v.ItineraryId == selectedVersion.ItineraryId
                    && v.CurrentUsageStatus == "啟用");

            if (currentVersion == null)
                throw new Exception("找不到目前版本");

            var model = new DiffViewModel
            {
                OldVersionId = selectedVersion.VersionId,
                NewVersionId = currentVersion.VersionId,
                OldContent = selectedVersion.Source,
                NewContent = currentVersion.Source,
                IsDifferent = selectedVersion.Source != currentVersion.Source
            };

            return model;
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

        public void SetCurrentVersion(int versionId)
        {
            var targetVersion = _versionRepo.GetById(versionId);

            if (targetVersion == null)
                throw new Exception("找不到版本");

            var itineraryId = targetVersion.ItineraryId;

            // 取得同一行程的所有版本
            var allVersions = _versionRepo.GetAll()
                .Where(v => v.ItineraryId == itineraryId)
                .ToList();

            // 先全部取消
            foreach (var version in allVersions)
            {
                version.CurrentUsageStatus = "非啟用"; // 或 false
                _versionRepo.Update(version);
            }

            // 設定指定版本
            targetVersion.CurrentUsageStatus = "啟用"; // 或 true
            _versionRepo.Update(targetVersion);

            _versionRepo.SaveChanges();
        }
    }
}

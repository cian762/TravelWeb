using CloudinaryDotNet.Actions;
using TravelWeb.Areas.Activity.Models.EFModel;
using TravelWeb.Areas.Activity.Models.ViewModels;

namespace TravelWeb.Areas.Activity.Repository.IActivityRepositories
{
    public interface IActivityTicketReposiotry
    {
        Task<IEnumerable<ActivityTicketViewModel>> GetAsync();

        Task<ActivityTicketViewModel> GetByProductCodeAsync(string ProductCode);

        Task<List<AcitivityTicket>> GetByActivityIdAsync(int activityId);

        Task CreateAsync(ActivityTicketViewModel vm);

        Task UpdateAsync(string ProductCode, ActivityTicketViewModel vm);

        Task SaveChangeAsync();



        Task<List<string>> TakeTicketCategoryNames();
    }
}

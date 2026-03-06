using CloudinaryDotNet.Actions;
using TravelWeb.Areas.Activity.Models.ViewModels;

namespace TravelWeb.Areas.Activity.Repository.IActivityRepositories
{
    public interface IActivityTicketReposiotry
    {
        Task<IEnumerable<ActivityTicketViewModel>> GetAsync();

        Task<ActivityTicketViewModel> GetByProductCodeAsync(string ProductCode);

        Task CreateAsync(ActivityTicketViewModel vm);

        Task UpdateAsync(ActivityTicketViewModel vm);

        Task SaveChangeAsync();

    }
}

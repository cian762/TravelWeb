using TravelWeb.Areas.Activity.Models.ViewModels;
using TravelWeb.Areas.Activity.Repository.IActivityRepositories;
using TravelWeb.Areas.Activity.Service.IActivityServices;

namespace TravelWeb.Areas.Activity.Service.ActivityServices
{
    public class ActivityTicketService : IActivityTicketService
    {

        private readonly IActivityTicketReposiotry _activityTicketReposiotry;
        public ActivityTicketService(IActivityTicketReposiotry activityTicketReposiotry)
        {
            _activityTicketReposiotry = activityTicketReposiotry;
        }

        public async Task<IEnumerable<ActivityTicketViewModel>> GetAllActTicketsAsync()
        {
            return await _activityTicketReposiotry.GetAsync();
        }

        public async Task<ActivityTicketViewModel> GetActTicketByProductCodeAsync(string ProductCode)
        {
            return await _activityTicketReposiotry.GetByProductCodeAsync(ProductCode);
        }

        public async Task CreateActTicketAsync(ActivityTicketViewModel vm)
        {
            throw new NotImplementedException();
        }

        public async Task EditActTicketAsync(ActivityTicketViewModel vm)
        {
            throw new NotImplementedException();
        }
    }
}

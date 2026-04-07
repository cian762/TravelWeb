using TravelWeb.Areas.Activity.Models.EFModel;
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

        public async Task<List<AcitivityTicket>> GetActTicketByActIdAsync(int activityId)
        {
            return await _activityTicketReposiotry.GetByActivityIdAsync(activityId);
        }


        public async Task<ActivityTicketViewModel> GetActTicketByProductCodeAsync(string ProductCode)
        {
            return await _activityTicketReposiotry.GetByProductCodeAsync(ProductCode);
        }

        public async Task CreateActTicketAsync(ActivityTicketViewModel vm)
        {
            await _activityTicketReposiotry.CreateAsync(vm);
            await _activityTicketReposiotry.SaveChangeAsync();
        }

        public async Task EditActTicketAsync(string ProductCode, ActivityTicketViewModel vm)
        {
            await _activityTicketReposiotry.UpdateAsync(ProductCode, vm);
            await _activityTicketReposiotry.SaveChangeAsync();
        }


        public async Task<List<string>> TakeTicketCategoryNames()
        {
            return await _activityTicketReposiotry.TakeTicketCategoryNames();
        }


    }
}

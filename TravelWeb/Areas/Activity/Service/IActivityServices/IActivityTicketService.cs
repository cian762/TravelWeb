using TravelWeb.Areas.Activity.Models.EFModel;
using TravelWeb.Areas.Activity.Models.ViewModels;

namespace TravelWeb.Areas.Activity.Service.IActivityServices
{
    public interface IActivityTicketService
    {
        Task<IEnumerable<ActivityTicketViewModel>> GetAllActTicketsAsync();

        Task<List<AcitivityTicket>> GetActTicketByActIdAsync(int activityId);

        Task<ActivityTicketViewModel> GetActTicketByProductCodeAsync(string ProductCode);

        Task CreateActTicketAsync(ActivityTicketViewModel vm);

        Task EditActTicketAsync(string ProductCode, ActivityTicketViewModel vm);

        //Task DeleteActTicket(int id);


        Task<List<string>> TakeTicketCategoryNames();
    }
}

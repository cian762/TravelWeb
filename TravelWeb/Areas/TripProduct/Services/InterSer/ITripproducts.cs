using TravelWeb.Areas.TripProduct.Models.ViewModels;

namespace TravelWeb.Areas.TripProduct.Services.InterSer
{
    public interface ITripproducts
    {
        Task<ViewModelProducts> GetCreateViewModelAsync();
        Task <int> Create(ViewModelProducts vm);
        
       Task <bool> Update(ViewModelProducts vm);
       Task <string> Delete(int id);
       Task<IEnumerable<TravelWeb.Areas.TripProduct.Models.TripProduct>> GetAll();
        Task<(IEnumerable<TripIndexViewModel> List, int TotalCount)> GetAllForIndex(string? keyword = null, int? regionId = null, string? status = null, int page = 1);
       Task<ViewModelProducts?> GetIdUpData(int id);
    }
}

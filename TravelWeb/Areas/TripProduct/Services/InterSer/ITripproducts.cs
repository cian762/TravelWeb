using TravelWeb.Areas.TripProduct.Models.ViewModels;

namespace TravelWeb.Areas.TripProduct.Services.InterSer
{
    public interface ITripproducts
    {
        Task<ViewModelProducts> GetCreateViewModelAsync();
        Task <bool> Create(ViewModelProducts vm);
        
       Task <bool> Update(ViewModelProducts vm);
       Task <bool> Delete(int id);
       
    }
}

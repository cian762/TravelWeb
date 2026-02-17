using TravelWeb.Areas.TripProduct.Models.ViewModels;

namespace TravelWeb.Areas.TripProduct.Services.InterSer
{
    public interface ITripItineraryItem
    {
        Task<IEnumerable<TravelWeb.Areas.TripProduct.Models.ViewModels.ViewModelTripItineraryItems> >IGetAny(int tripProductId);
        Task <bool> ICreate(TravelWeb.Areas.TripProduct.Models.ViewModels.ViewModelTripItineraryItems items);
        Task<bool> IUpdate(TravelWeb.Areas.TripProduct.Models.ViewModels.ViewModelTripItineraryItems items);
        Task<bool> IDelete(int id);
        Task<ViewModelTripItineraryItems> PrepareViewModel(int tripProductId);
    }
}

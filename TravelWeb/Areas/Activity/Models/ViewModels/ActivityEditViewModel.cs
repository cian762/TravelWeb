using TravelWeb.Areas.Activity.Models.EFModel;

namespace TravelWeb.Areas.Activity.Models.ViewModels
{
    public class ActivityEditViewModel
    {
        public Activity.Models.EFModel.Activity? ActivityInfo { get; set; }

        public List<int>? RegoinID { get; set; }

        public List<int>? TypeID { get; set; }

    }
}

using TravelWeb.Areas.Activity.Models.EFModel;

namespace TravelWeb.Areas.Activity.Models.ViewModels
{
    public class ActivityEditViewModel
    {
        public required Activity.Models.EFModel.Activity ActivityInfo { get; set; }

        public required List<string> RegoinName { get; set; } = new List<string>();

        public required List<string> TypeName { get; set; } = new List<string>();


    }
}

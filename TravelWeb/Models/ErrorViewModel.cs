using System.ComponentModel.DataAnnotations.Schema;

namespace TravelWeb.Models
{

    [Table("ErrorViewModel", Schema = "Member")]
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}

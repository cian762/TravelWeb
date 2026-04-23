using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.BoardManagement.Models.BoardDB;
using static TravelWeb.Areas.BoardManagement.Models.ViewModel.ReportDetailsModel;

namespace TravelWeb.Areas.BoardManagement.Models.IService
{
    public interface INoteService
    {

        public void AddNote(int id);
        public AuditNote GetNote(int id);
        public List<LogItem> GetReportLogs(int id);
        public void UpdateNote(int id, string note);

        public void UpdateReportLog(int LogID, int ResultID);

        public void AddReport(ReportLog report);      


        public void SaveChange();

    }
}

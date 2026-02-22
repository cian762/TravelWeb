using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TravelWeb.Areas.BoardManagement.Data;
using TravelWeb.Areas.BoardManagement.Models.BoardDB;
using TravelWeb.Areas.BoardManagement.Models.IService;
using TravelWeb.Areas.BoardManagement.Models.ViewModel;
using static TravelWeb.Areas.BoardManagement.Models.ViewModel.ReportDetailsModel;

namespace TravelWeb.Areas.BoardManagement.Models.Service
{
    public class NoteService:INoteService
    {
        private readonly BoardDbContext _context;
       

        
       
        public NoteService(BoardDbContext context)
        {
            _context = context;
            
        }

        public void AddNote(int id)
        {
            var note = new AuditNote { TargetId = id, Note = "" };
            _context.AuditNotes.Add(note);
            SaveChange();
            
        }

        public void AddReport(ReportLog report)
        {
            var log = _context.ReportLogs.Add(report);
            SaveChange();
        }

        public AuditNote GetNote(int id)
        {
            var note = _context.AuditNotes.FirstOrDefault(n=>n.TargetId==id);
            if (note == null)
            {
                AddNote(id);
                return new AuditNote {TargetId=id,Note=""};
            }

            return note;
        }

        public List<LogItem> GetReportLogs(int id)
        {
            var logs = _context.ReportLogs.Where(r => r.TargetId == id);
            List<LogItem> logItems = logs.Select(r => new LogItem
            {
                LogId = r.LogId,
                UserId = r.UserId,
                ViolationType = StaticConstants.ViolationName[r.ViolationType],
                ResultType = r.ResultType,
                Snapshot = r.Snapshot,
                Photo = r.Photo,
                Reason = r.Reason,
                UpdatedAt = r.UpdatedAt,
                CreatedAt = r.CreatedAt,
            }).ToList();

            return logItems;
        }
            

        public void SaveChange()
        {
            _context.SaveChanges();
        }

        public void UpdateNote(int id, string note)
        {
            var auditNote = _context.AuditNotes.Find(id);
            if (auditNote != null)
            {
                auditNote.Note = note;                
            }

            SaveChange();
        }

        public void UpdateReportLog(int LogID, int ResultID)
        {
            var Log = _context.ReportLogs.Find(LogID);
            if (Log == null)
                return;
            Log.ResultType = (byte)ResultID;
            Log.UpdatedAt = DateTime.Now;
            SaveChange();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AccessPointControlClient.DTO
{
    public class LogsDTO
    {
        public LogEntry SelectedEntry { get; private set; }
        public List<LogEntry> LogContent { get; set; }

        public LogsDTO()
        {
            LogContent = new List<LogEntry>();
        }

        public void SetSelectedEntry(LogEntry entry)
        {
            SelectedEntry = new LogEntry();
            SelectedEntry.FileName = entry.FileName;
            //SelectedEntry.FileContent = entry.FileContent.Replace(Environment.NewLine, "<br />");
            SelectedEntry.FileContent = entry.FileContent;
        }
    }

    public class LogEntry 
    {
        public string FileName { get; set; }
        public string FileContent { get; set; }
    }
}

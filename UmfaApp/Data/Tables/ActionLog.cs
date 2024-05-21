using UmfaApp.Helpers;
using Action = UmfaApp.Enums.Action;

namespace UmfaApp.Data.Tables
{
    public class ActionLog : TableBase
    {
        public string Action {  get; set; }
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public string ActionData {  get; set; }
        public string ErrorMessage { get; set; }

        public Guid ReadingListId { get; set; }

        public ActionLog() { }

        public ActionLog(Action action, int userId, string data, Guid? readingListId, string error) 
        {
            Action = action.GetEnumDisplayName();
            ActionDate = DateTime.UtcNow;
            UserId = userId;
            ActionData = data;
            ReadingListId = readingListId ?? Guid.Empty;
            ErrorMessage = error;
        }
    }
}

namespace DDNAEINV.Model.Views
{
    public class ActivityLogVw
    {
        public int? ActivityID { get; set; } = 0;
        public string? TableName { get; set; } = String.Empty;
        public string? ActionType { get; set; } = String.Empty;
        public string? RecordID { get; set; } = String.Empty;
        public string? Description { get; set; } = String.Empty;
        public string? Details { get; set; } = String.Empty;
        public string? ActionBy { get; set; } = String.Empty;
        public required DateTime ActivityDate { get; set; }
        public string? ActionByName { get; set; } = String.Empty;
    }
}

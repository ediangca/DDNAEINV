using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDNAEINV.Model.Views
{
    public class OPTRVw
    {
        [Key]
        public string OPTRNo { get; set; } = string.Empty;
        public required int OPRNo { get; set; }
        public required string itemSource { get; set; }
        public required string ownership { get; set; }
        public string? ttype { get; set; } = string.Empty.ToString();
        public string? otype { get; set; } = string.Empty.ToString();
        public string? reason { get; set; } = string.Empty.ToString();
        public required string receivedBy { get; set; }
        public required string received { get; set; }
        public required string issuedBy { get; set; }
        public required string issued { get; set; }
        public string? approvedBy { get; set; } = string.Empty.ToString();
        public string? approved { get; set; } = string.Empty.ToString();
        public bool? postFlag { get; set; } = false;
        public bool? voidFlag { get; set; } = false;
        public required string createdBy { get; set; }
        public required string created { get; set; }
        public required DateTime Date_Created { get; set; }
        public required DateTime Date_Updated { get; set; }

    }
}

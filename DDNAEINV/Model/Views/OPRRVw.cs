using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDNAEINV.Model.Entities
{
    public class OPRRVw
    {
        [Key]
        public required string OPRRNo { get; set; }
        public required string rtype { get; set; }
        public required string otype { get; set; }
        public string? receivedBy { get; set; } = string.Empty.ToString();
        public string? received { get; set; } = string.Empty.ToString();
        public string? issuedBy { get; set; } = string.Empty.ToString();
        public string? issued { get; set; } = string.Empty.ToString();
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

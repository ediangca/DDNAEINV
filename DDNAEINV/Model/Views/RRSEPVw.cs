using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDNAEINV.Model.Entities
{
    public class RRSEPVw
    {
        [Key]
        public required string RRSEPNo { get; set; }
        public string? entityName { get; set; } = string.Empty;
        public string? rtype { get; set; } = string.Empty;
        public string? otype { get; set; } = string.Empty;
        public string? receivedBy { get; set; } = string.Empty;
        public string? received { get; set; } = string.Empty;
        public string? issuedBy { get; set; } = string.Empty;
        public string? issued { get; set; } = string.Empty;
        public string? approvedBy { get; set; } = string.Empty;
        public string? approved { get; set; } = string.Empty;
        public bool? postFlag { get; set; } = false;
        public bool? voidFlag { get; set; } = false;
        public required string createdBy { get; set; }
        public required string created { get; set; }
        public required DateTime Date_Created { get; set; }
        public required DateTime Date_Updated { get; set; }

    }
}

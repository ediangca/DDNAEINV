using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDNAEINV.Model.Details
{
    public class OPTRDto
    {
        [Key]

        public string? OPTRNo { get; set; } = string.Empty;
        public required int oprNo { get; set; }
        public required string ttype { get; set; }
        public required string otype { get; set; }
        public required string reason { get; set; }
        public required string receivedBy { get; set; }
        public required string issuedBy { get; set; }
        public string? approvedBy { get; set; } = string.Empty.ToString();
        public bool? postFlag { get; set; } = false;
        public bool? voidFlag { get; set; } = false;
        public required string createdBy { get; set; }

    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDNAEINV.Model.Entities
{
    public class RePAR
    {
        [Key]
        public required string REPARNo { get; set; }
        public required string parNo { get; set; }
        public required string ttype { get; set; }
        public required string otype { get; set; }
        public required string reason { get; set; }
        public required string receivedBy { get; set; }
        public required string issuedBy { get; set; }
        public string? approvedBy { get; set; } = string.Empty.ToString();
        public bool? postFlag { get; set; } = false;
        public bool? voidFlag { get; set; } = false;
        public required string createdBy { get; set; }
        public required DateTime Date_Created { get; set; }
        public required DateTime Date_Updated { get; set; }

    }
}

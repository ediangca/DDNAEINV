using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDNAEINV.Model.Views
{
    public class ITR
    {
        [Key]
        public string ITRNo { get; set; } = string.Empty;
        public required string icsNo { get; set; }
        public required string ttype { get; set; }
        public required string otype { get; set; }
        public required string reason { get; set; }
        public required string receivedBy { get; set; }
        public required string issuedBy { get; set; }
        public required string approvedBy { get; set; }
        public bool? postFlag { get; set; } = false;
        public bool? voidFlag { get; set; } = false;
        public required string createdBy { get; set; }
        public required DateTime Date_Created { get; set; }
        public required DateTime Date_Updated { get; set; }

    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDNAEINV.Model.Details
{
    public class ICSDto
    {
        [Key]
        public required string ICSNo { get; set; }
        public required string entityName { get; set; }
        public required string fund { get; set; }
        public required string receivedBy { get; set; }
        public required string issuedBy { get; set; }
        public bool? postFlag { get; set; } = false;
        public bool? voidFlag { get; set; } = false;
        public required string createdBy { get; set; }



    }
}

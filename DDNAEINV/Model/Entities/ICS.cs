using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDNAEINV.Model.Entities
{
    public class ICS
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
        public DateTime? Date_Created { get; set; } = DateTime.Now;
        public DateTime? Date_Updated { get; set; } = DateTime.Now;



    }
}

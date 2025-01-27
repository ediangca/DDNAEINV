using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDNAEINV.Model.Entities
{
    public class OPRVw
    {
        [Key]
        public required int oprNo { get; set; }
        public required string itemSource { get; set; }
        public required string ownership { get; set; }
        public required string receivedBy { get; set; }
        public required string received { get; set; }
        public required string issuedBy { get; set; }
        public required string issued { get; set; }
        public bool? postFlag { get; set; } = false;
        public bool? voidFlag { get; set; } = false;
        public required string createdBy { get; set; }
        public required string created { get; set; }
        public required DateTime Date_Created { get; set; }
        public required DateTime Date_Updated { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDNAEINV.Model.Entities
{
    public class OPRDto
    {
        public int? oprNo { get; set; } = 0;
        public required string itemSource { get; set; }
        public required string ownership { get; set; }
        public required string receivedBy { get; set; }
        public required string issuedBy { get; set; }
        public bool? postFlag { get; set; } = false;
        public bool? voidFlag { get; set; } = false;
        public required string createdBy { get; set; }
    }
}

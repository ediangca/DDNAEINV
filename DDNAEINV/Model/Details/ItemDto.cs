using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDNAEINV.Model.Details
{
    public class ItemDto
    {
        [Key]
        public required string  IID { get; set; }
        public required int IGID { get; set; }
        public required string Description { get; set; }

    }
}

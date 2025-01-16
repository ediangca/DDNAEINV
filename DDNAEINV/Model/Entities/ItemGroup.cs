using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDNAEINV.Model.Entities
{
    public class ItemGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IGID { get; set; }
        public required string ItemGroupName { get; set; }
        public required string Notes { get; set; }
        public bool? isDefault { get; set; } = false;
        public required DateTime Date_Created { get; set; }
        public required DateTime Date_Updated { get; set; }
    }
}

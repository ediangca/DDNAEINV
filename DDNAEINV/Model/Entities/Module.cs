using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace DDNAEINV.Model.Entities
{
    public class Module
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? MID { get; set; } = 0;
        public required string ModuleName { get; set; }
    }
}

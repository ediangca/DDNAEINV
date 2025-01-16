using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDNAEINV.Model.Entities
{
    public class Section
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SecID { get; set; }
        public required string SectionName { get; set; }
        public required int DepID { get; set; }
        public required DateTime Date_Created { get; set; }
        public required DateTime Date_Updated { get; set; }
    }
}

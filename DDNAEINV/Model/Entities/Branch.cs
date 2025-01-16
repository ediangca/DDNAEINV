using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDNAEINV.Model.Entities
{
    public class Branch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BranchID { get; set; }
        public  string? BranchName { get; set; } = string.Empty;
        public required string Type { get; set; }
        public required DateTime Date_Created { get; set; }
        public required DateTime Date_Updated { get; set; }

    }
}

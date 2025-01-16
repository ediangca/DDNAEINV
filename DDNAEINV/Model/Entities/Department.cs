using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDNAEINV.Model.Entities
{
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DepID { get; set; }
        public required string DepartmentName { get; set; }
        public required int BranchID { get; set; }
        public required DateTime Date_Created { get; set; }
        public required DateTime Date_Updated { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace DDNAEINV.Model.Entities
{
    public class PrivilegeVw
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? PID { get; set; } = 0;
        public required int UGID { get; set; }
        public required string UserGroupName { get; set; }
        public required int MID { get; set; }
        public required string ModuleName { get; set; }
        public bool? isActive { get; set; } = false;
        public bool? C { get; set; } = false;
        public bool? R { get; set; } = false; //AS WILL AS VIEW
        public bool? U { get; set; } = false;
        public bool? D { get; set; } = false;
        public bool? POST { get; set; } = false;
        public bool? UNPOST { get; set; } = false;
    }
}

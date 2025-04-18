using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;
using System;

namespace DDNAEINV.Model.Entities
{
    public class Leave
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? LeaveID { get; set; } = null;
        public required string UserID { get; set; }
        public required string Remarks { get; set; }
        public required string CareOfUserID { get; set; }
        public DateTime? Date_Created { get; set; }

    }
}

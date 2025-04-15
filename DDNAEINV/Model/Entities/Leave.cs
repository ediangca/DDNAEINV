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
        public int? LeaveID { get; set; } = 0;
        public required string UserID { get; set; }
        public required string Remarks { get; set; }
        public required string CareOfID { get; set; }
        public required DateTime Date_Created { get; set; }

    }
}

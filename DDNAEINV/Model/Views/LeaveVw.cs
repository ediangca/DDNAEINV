using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace DDNAEINV.Model.Entities
{
    public class LeaveVw
    {
        public int leaveID { get; set; }
        public string UserID { get; set; }
        public string Fullname { get; set; }
        public string Remarks { get; set; }
        public string CareOfUserID { get; set; }
        public string CareOfUser { get; set; }
        public DateTime? Date_Created { get; set; } = DateTime.Now;


    }
}

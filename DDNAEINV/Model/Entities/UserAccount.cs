using System.ComponentModel.DataAnnotations;

namespace DDNAEINV.Model.Entities
{
    public class UserAccount
    {
        [Key]
        public string? UserID { get; set; } = string.Empty;
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required int UGID { get; set; }
        public string? Token { get; set; }
        public bool? isVerified { get; set; } = false;
        public required DateTime Date_Created { get; set; }
        public required DateTime Date_Updated { get; set; }
    }
}

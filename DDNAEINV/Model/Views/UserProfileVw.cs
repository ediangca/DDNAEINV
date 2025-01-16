using DDNAEINV.Model.Entities;

namespace DDNAEINV.Model.Views
{
    public class UserProfileVw
    {
        //ProfileID, Lastname, Firstname, Middlename, Branch, Department, Section, Position, UserID, Date_Created, Date_Updated
        public int ProfileID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
        public string? Branch { get; set; } = string.Empty;
        public string? Department { get; set; } = string.Empty;
        public string? Section { get; set; } = string.Empty;
        public string? Position { get; set; } = string.Empty;
        public string UserID { get; set; } = string.Empty;
        public DateTime Date_Created { get; set; }
        public DateTime Date_Updated { get; set; }

    }
}

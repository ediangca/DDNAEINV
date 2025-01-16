namespace DDNAEINV.Model.Entities
{
    public class UserProfile
    {
        public int ProfileID { get; set; }
        public required string Lastname { get; set; }
        public required string Firstname { get; set; }
        public required string Middlename { get; set; }
        public required string Sex { get; set; }
        public int? BranchID { get; set; } = null;
        public int? DepID { get; set; } = null;
        public int? SecID { get; set; } = null;
        public int? PositionID { get; set; } = null;
        public required string UserID { get; set; }
        public required DateTime Date_Created { get; set; }
        public required DateTime Date_Updated { get; set; }

    }
}

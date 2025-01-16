namespace DDNAEINV.Model.Entities
{
    public class UserProfileDto
    {
        public required string Lastname { get; set; }
        public required string Firstname { get; set; }
        public required string Middlename { get; set; }
        public string Sex { get; set; }
        public int? BranchID { get; set; } = null;
        public int? DepID { get; set; } = null;
        public int? SecID { get; set; } = null;
        public int? PositionID { get; set; } = null;
        public string? UserID { get; set; } = null;

    }
}

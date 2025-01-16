namespace DDNAEINV.Model.Entities
{
    public class BranchType
    {
        public int BTID { get; set; } // Primary key
        public required string Type { get; set; }
        public required DateTime Date_Created { get; set; }
        public required DateTime Date_Updated { get; set; }

    }
}

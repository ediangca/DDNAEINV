namespace DDNAEINV.Model.Views
{
    public class SectionsVw
    {
        public int SecID { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int DepID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public DateTime Date_Created { get; set; }
        public DateTime Date_Updated { get; set; }
    }
}

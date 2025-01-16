namespace DDNAEINV.Model.Views
{
    public class CencusVw
    {

        //TotalPAR TotalREPAR  TotalITR TotalICS
        public required int TotalPAR { get; set; }
        public required int TotalREPAR { get; set; }
        public required int TotalPRS { get; set; }
        public required int TotalITR { get; set; }
        public required int TotalICS { get; set; }
        public required int TotalRRSEP { get; set; }
        public required int TotalItemsAbove50 { get; set; }
        public required int TotalItemsBelow50 { get; set; }
        public required int TotalUsers { get; set; }
    }
}

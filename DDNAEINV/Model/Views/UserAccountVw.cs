namespace DDNAEINV.Model.Views
{
    public class UserAccountsVw
    {
        public string UserID { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Fullname { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? UGID { get; set; } = null;
        public string UserGroupName { get; set; } = string.Empty;
        public string? Token { get; set; }
        public bool? isVerified { get; set; } = false;
        public bool? isLeave { get; set; } = false;
        public DateTime Date_Created { get; set; }
        public DateTime Date_Updated { get; set; }
    }
}

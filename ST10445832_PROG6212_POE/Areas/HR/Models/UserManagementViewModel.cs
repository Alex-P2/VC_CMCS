namespace ST10445832_PROG6212_POE.Areas.HR.Models
{
    public class UserManagementViewModel
    {
        public bool Filtered { get; set; }
        public string Filter { get; set; } = string.Empty;
        public string Query { get; set; } = string.Empty;
        public List<UserDetail> Users { get; set; } = [];
    }
}
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
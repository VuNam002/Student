namespace Student_management.DTOs.Account
{
    public class AccountSearch
    {
        public string? Keyword { get; set; }
        public byte? Status { get; set; }
        public int Page { get; set; } = 1; // Giá trị mặc định
        public int PageSize { get; set; } = 10; // Giá trị mặc định
    }
}

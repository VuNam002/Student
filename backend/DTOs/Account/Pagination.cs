namespace Student_management.DTOs.Account
{
    public class Pagination
    {
        public List<AccountDto> Account { get; set; } = new List<AccountDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}

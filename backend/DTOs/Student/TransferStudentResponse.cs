namespace Student_management.DTOs.Student
{
    public class TransferStudentResponse
    {
        public string Message { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public int? OldClassId { get; set; }
        public string? OldClassName { get; set; }
        public int NewClassId { get; set; }
        public string NewClassName { get; set; } = string.Empty;
    }
}

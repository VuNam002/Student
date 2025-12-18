namespace Student_management.DTOs.Teacher
{
    public class CreateTeacher
    {
        public string TeacherCode { get; set; }
        public int PersonID { get; set; }
        public int DepartmentID { get; set; }
        public string Position { get; set; }
        public string Specialization { get; set; }
        public int AccountID { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}

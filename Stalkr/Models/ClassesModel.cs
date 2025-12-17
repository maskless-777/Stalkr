namespace Stalkr.Models
{
    public class ClassesModel
    {
        public required int CourseID { get; set; }
        public required string ClassName { get; set; } = string.Empty;
        public required string CourseTerm { get; set; } = string.Empty;
    }
}

namespace Stalkr.Models
{
    public class PrereqRelationshipModel
    {
        public string FromCourseID { get; set; } = string.Empty;  
        public string ToCourseID { get; set; } = string.Empty;   

        public ClassesModel? From { get; set; }
        public ClassesModel? To { get; set; }
    }
}

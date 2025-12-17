namespace Stalkr.Models
{
    public class EnrolledInRelationshipModel
    {
        public int PersonID { get; set; }     
        public string CourseID { get; set; } = string.Empty;   

        public PeopleModel? Person { get; set; }
        public ClassesModel? Class { get; set; }
    }
}

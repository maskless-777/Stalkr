namespace Stalkr.Models
{
    public class EnjoysRelationshipModel
    {
        public int PersonID { get; set; }           
        public int HobbyID { get; set; }            
        public PeopleModel? Person { get; set; }
        public HobbyModel? Hobby { get; set; }
    }
}

namespace Stalkr.Models
{
    public class KnowsRelationshipModel
    {
        public int FromPersonID { get; set; }
        public int ToPersonID { get; set; }

        public PeopleModel? From { get; set; }
        public PeopleModel? To { get; set; }
    }
}

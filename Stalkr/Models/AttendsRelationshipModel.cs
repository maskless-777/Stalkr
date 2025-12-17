using Stalkr.Repositories;

namespace Stalkr.Models
{
    public class AttendsRelationshipModel
    {
        public int PersonID { get; set; }
        public int SchoolID { get; set; }

        public PeopleModel? Person { get; set; }
        public SchoolsModel? School { get; set; }
    }
}

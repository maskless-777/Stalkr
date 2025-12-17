namespace Stalkr.Models
{
    public class Majors_InModel
    {
        public int PersonID { get; set; }
        public int MajorID { get; set; }

        public PeopleModel? Person { get; set; }
        public MajorModel? Major { get; set; }
    }
}

namespace Stalkr.Models
{
    public class PeopleModel
    {
        public required int PersonID { get; set; }
        public required string FirstName { get; set; } = string.Empty;
        public required string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}

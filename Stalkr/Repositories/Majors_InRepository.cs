using Neo4j.Driver;
using Stalkr.Models;

namespace Stalkr.Repositories
{
    public class Majors_InRepository
    {
        private readonly IDriver _driver;

        public Majors_InRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<bool> CreateAsync(Majors_InModel rel)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (a:People {PersonID: $PersonId}), (b:Majors {MajorID: $mahorID})
                MERGE (a)-[r:MAJORS_IN]->(b)
                RETURN r",
                new { PersonId = rel.PersonID, mahorID = rel.MajorID }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<bool> DeleteAsync(int personID, int majorID)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (a:People {PersonID: $personID})-[r:MAJORS_IN]->(b:Majors {MajorID: $majorID})
                DELETE r
                RETURN r",
                new { personID, majorID }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<IEnumerable<Majors_InModel>> GetAllAsync()
        {
            var list = new List<Majors_InModel>();
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (a:People)-[r:MAJORS_IN]->(b:Majors)
                RETURN a, b"
            );

            var records = await cursor.ToListAsync();
            foreach (var record in records)
            {
                var personNode = record["a"].As<INode>();
                var majorNode = record["b"].As<INode>();

                list.Add(new Majors_InModel
                {
                    PersonID = personNode.Properties["PersonID"].As<int>(),
                    MajorID = majorNode.Properties["MajorID"].As<int>(),
                    Person = new PeopleModel
                    {
                        PersonID = personNode.Properties["PersonID"].As<int>(),
                        FirstName = personNode.Properties["FirstName"].As<string>(),
                        LastName = personNode.Properties["LastName"].As<string>(),
                        Age = personNode.Properties["Age"].As<int>()
                    },
                    Major = new MajorModel
                    {
                        MajorID = majorNode.Properties["MajorID"].As<int>(),
                        MajorName = majorNode.Properties["MajorName"].As<string>()

                    }
                });
            }

            return list;
        }

        public async Task<IEnumerable<Majors_InModel>> GetByFromPersonIdAsync(int personID)
        {
            var list = new List<Majors_InModel>();
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
        MATCH (a:People {PersonID: $personID})-[r:MAJORS_IN]->(b:Majors)
        RETURN a, b",
                new { personID = personID }
            );

            var records = await cursor.ToListAsync();
            foreach (var record in records)
            {
                var person = record["a"].As<INode>();
                var major = record["b"].As<INode>();

                list.Add(new Majors_InModel
                {
                    PersonID = person.Properties["PersonID"].As<int>(),
                    MajorID = major.Properties["MajorID"].As<int>(),
                    Person = new PeopleModel
                    {
                        PersonID = person.Properties["PersonID"].As<int>(),
                        FirstName = person.Properties["FirstName"].As<string>(),
                        LastName = person.Properties["LastName"].As<string>(),
                        Age = person.Properties["Age"].As<int>()
                    },
                    Major = new MajorModel
                    {
                        MajorID = major.Properties["MajorID"].As<int>(),
                        MajorName = major.Properties["MajorName"].As<string>()
                    }
                });
            }

            return list;
        }
    }
}

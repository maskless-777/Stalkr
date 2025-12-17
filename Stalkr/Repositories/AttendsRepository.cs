using Neo4j.Driver;
using Stalkr.Models;

namespace Stalkr.Repositories
{
    public class AttendsRepository
    {
        private readonly IDriver _driver;

        public AttendsRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<bool> CreateAsync(AttendsRelationshipModel rel)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (a:People {PersonID: $PersonId}), (b:Schools {SchoolID: $schoolID})
                MERGE (a)-[r:ATTENDS]->(b)
                RETURN r",
                new { PersonId = rel.PersonID, schoolID = rel.SchoolID }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<bool> DeleteAsync(int personID, int schoolID)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (a:People {PersonID: $personID})-[r:ATTENDS]->(b:Schools {SchoolID: $schoolID})
                DELETE r
                RETURN r",
                new { personID, schoolID }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<IEnumerable<AttendsRelationshipModel>> GetAllAsync()
        {
            var list = new List<AttendsRelationshipModel>();
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (a:People)-[r:ATTENDS]->(b:Schools)
                RETURN a, b"
            );

            var records = await cursor.ToListAsync();
            foreach (var record in records)
            {
                var personNode = record["a"].As<INode>();
                var schoolNode = record["b"].As<INode>();

                list.Add(new AttendsRelationshipModel
                {
                    PersonID = personNode.Properties["PersonID"].As<int>(),
                    SchoolID = schoolNode.Properties["SchoolID"].As<int>(),
                    Person = new PeopleModel
                    {
                        PersonID = personNode.Properties["PersonID"].As<int>(),
                        FirstName = personNode.Properties["FirstName"].As<string>(),
                        LastName = personNode.Properties["LastName"].As<string>(),
                        Age = personNode.Properties["Age"].As<int>()
                    },
                    School = new SchoolsModel
                    {
                        SchoolID = schoolNode.Properties["SchoolID"].As<int>(),
                        SchoolName = schoolNode.Properties["SchoolName"].As<string>()

                    }
                });
            }

            return list;
        }

        public async Task<IEnumerable<AttendsRelationshipModel>> GetByFromPersonIdAsync(int personID)
        {
            var list = new List<AttendsRelationshipModel>();
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
        MATCH (a:People {PersonID: $personID})-[r:ATTENDS]->(b:Schools)
        RETURN a, b",
                new { personID = personID }
            );

            var records = await cursor.ToListAsync();
            foreach (var record in records)
            {
                var fromNode = record["a"].As<INode>();
                var toNode = record["b"].As<INode>();

                list.Add(new AttendsRelationshipModel
                {
                    PersonID = fromNode.Properties["PersonID"].As<int>(),
                    SchoolID = toNode.Properties["SchoolID"].As<int>(),
                    Person = new PeopleModel
                    {
                        PersonID = fromNode.Properties["PersonID"].As<int>(),
                        FirstName = fromNode.Properties["FirstName"].As<string>(),
                        LastName = fromNode.Properties["LastName"].As<string>(),
                        Age = fromNode.Properties["Age"].As<int>()
                    },
                    School = new SchoolsModel
                    {
                        SchoolID = toNode.Properties["SchoolID"].As<int>(),
                        SchoolName = toNode.Properties["SchoolName"].As<string>()
                    }
                });
            }

            return list;
        }
    }
}

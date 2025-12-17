using Neo4j.Driver;
using Stalkr.Models;

namespace Stalkr.Repositories
{
    public class EnjoysRepository
    {
        private readonly IDriver _driver;

        public EnjoysRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<bool> CreateAsync(EnjoysRelationshipModel rel)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (p:People {PersonID: $personId}), (h:Hobbies {HobbyID: $hobbyId})
                MERGE (p)-[r:ENJOYS]->(h)
                RETURN r",
                new { personId = rel.PersonID, hobbyId = rel.HobbyID }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<bool> DeleteAsync(int personId, int hobbyId)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (p:People {PersonID: $personId})-[r:ENJOYS]->(h:Hobbies {HobbyID: $hobbyId})
                DELETE r
                RETURN r",
                new { personId, hobbyId }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<IEnumerable<EnjoysRelationshipModel>> GetAllAsync()
        {
            var list = new List<EnjoysRelationshipModel>();
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (p:People)-[r:ENJOYS]->(h:Hobbies)
                RETURN p, h"
            );

            var records = await cursor.ToListAsync();
            foreach (var record in records)
            {
                var personNode = record["p"].As<INode>();
                var hobbyNode = record["h"].As<INode>();

                list.Add(new EnjoysRelationshipModel
                {
                    PersonID = personNode.Properties["PersonID"].As<int>(),
                    HobbyID = hobbyNode.Properties["HobbyID"].As<int>(),
                    Person = new PeopleModel
                    {
                        PersonID = personNode.Properties["PersonID"].As<int>(),
                        FirstName = personNode.Properties["FirstName"].As<string>(),
                        LastName = personNode.Properties["LastName"].As<string>(),
                        Age = personNode.Properties["Age"].As<int>()
                    },
                    Hobby = new HobbyModel
                    {
                        HobbyID = hobbyNode.Properties["HobbyID"].As<int>(),
                        HobbyName = hobbyNode.Properties["HobbyName"].As<string>()
                    }
                });
            }

            return list;
        }

        public async Task<IEnumerable<EnjoysRelationshipModel>> GetByPersonIdAsync(int personId)
        {
            var list = new List<EnjoysRelationshipModel>();
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (p:People {PersonID: $personId})-[r:ENJOYS]->(h:Hobbies)
                RETURN p, h",
                new { personId }
            );

            var records = await cursor.ToListAsync();
            foreach (var record in records)
            {
                var personNode = record["p"].As<INode>();
                var hobbyNode = record["h"].As<INode>();

                list.Add(new EnjoysRelationshipModel
                {
                    PersonID = personNode.Properties["PersonID"].As<int>(),
                    HobbyID = hobbyNode.Properties["HobbyID"].As<int>(),
                    Person = new PeopleModel
                    {
                        PersonID = personNode.Properties["PersonID"].As<int>(),
                        FirstName = personNode.Properties["FirstName"].As<string>(),
                        LastName = personNode.Properties["LastName"].As<string>(),
                        Age = personNode.Properties["Age"].As<int>()
                    },
                    Hobby = new HobbyModel
                    {
                        HobbyID = hobbyNode.Properties["HobbyID"].As<int>(),
                        HobbyName = hobbyNode.Properties["HobbyName"].As<string>()
                    }
                });
            }

            return list;
        }
    }
}

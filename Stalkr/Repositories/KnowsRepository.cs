using Neo4j.Driver;
using Stalkr.Models;

namespace Stalkr.Repositories
{
    public class KnowsRepository
    {
        private readonly IDriver _driver;

        public KnowsRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<bool> CreateAsync(KnowsRelationshipModel rel)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (a:People {PersonID: $fromId}), (b:People {PersonID: $toId})
                MERGE (a)-[r:KNOWS]->(b)
                RETURN r",
                new { fromId = rel.FromPersonID, toId = rel.ToPersonID }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<bool> DeleteAsync(int fromId, int toId)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (a:People {PersonID: $fromId})-[r:KNOWS]->(b:People {PersonID: $toId})
                DELETE r
                RETURN r",
                new { fromId, toId }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<IEnumerable<KnowsRelationshipModel>> GetAllAsync()
        {
            var list = new List<KnowsRelationshipModel>();
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (a:People)-[r:KNOWS]->(b:People)
                RETURN a, b"
            );

            var records = await cursor.ToListAsync();
            foreach (var record in records)
            {
                var fromNode = record["a"].As<INode>();
                var toNode = record["b"].As<INode>();

                list.Add(new KnowsRelationshipModel
                {
                    FromPersonID = fromNode.Properties["PersonID"].As<int>(),
                    ToPersonID = toNode.Properties["PersonID"].As<int>(),
                    From = new PeopleModel
                    {
                        PersonID = fromNode.Properties["PersonID"].As<int>(),
                        FirstName = fromNode.Properties["FirstName"].As<string>(),
                        LastName = fromNode.Properties["LastName"].As<string>(),
                        Age = fromNode.Properties["Age"].As<int>()
                    },
                    To = new PeopleModel
                    {
                        PersonID = toNode.Properties["PersonID"].As<int>(),
                        FirstName = toNode.Properties["FirstName"].As<string>(),
                        LastName = toNode.Properties["LastName"].As<string>(),
                        Age = toNode.Properties["Age"].As<int>()
                    }
                });
            }

            return list;
        }

        public async Task<IEnumerable<KnowsRelationshipModel>> GetByFromPersonIdAsync(int fromPersonId)
        {
            var list = new List<KnowsRelationshipModel>();
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
        MATCH (a:People {PersonID: $fromId})-[r:KNOWS]->(b:People)
        RETURN a, b",
                new { fromId = fromPersonId }
            );

            var records = await cursor.ToListAsync();
            foreach (var record in records)
            {
                var fromNode = record["a"].As<INode>();
                var toNode = record["b"].As<INode>();

                list.Add(new KnowsRelationshipModel
                {
                    FromPersonID = fromNode.Properties["PersonID"].As<int>(),
                    ToPersonID = toNode.Properties["PersonID"].As<int>(),
                    From = new PeopleModel
                    {
                        PersonID = fromNode.Properties["PersonID"].As<int>(),
                        FirstName = fromNode.Properties["FirstName"].As<string>(),
                        LastName = fromNode.Properties["LastName"].As<string>(),
                        Age = fromNode.Properties["Age"].As<int>()
                    },
                    To = new PeopleModel
                    {
                        PersonID = toNode.Properties["PersonID"].As<int>(),
                        FirstName = toNode.Properties["FirstName"].As<string>(),
                        LastName = toNode.Properties["LastName"].As<string>(),
                        Age = toNode.Properties["Age"].As<int>()
                    }
                });
            }

            return list;
        }

    }
}

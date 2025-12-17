using Neo4j.Driver;
using Stalkr.Models;

namespace Stalkr.Repositories
{
    public class SchoolRepository : IRepository<SchoolsModel>
    {
        private readonly IDriver _driver;

        public SchoolRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<IEnumerable<SchoolsModel>> GetAllAsync()
        {
            var people = new List<SchoolsModel>();

            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var cursor = await session.RunAsync("MATCH (n:Schools) RETURN n");
            var records = await cursor.ToListAsync();

            foreach (var record in records)
            {
                var node = record["n"].As<INode>();
                var person = new SchoolsModel
                {
                    SchoolID = node.Properties["SchoolID"].As<int>(),
                    SchoolName = node.Properties["SchoolName"].As<string>(),
                };
                people.Add(person);
            }

            return people;
        }

        public async Task<SchoolsModel?> FindByIdAsync(int id)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var cursor = await session.RunAsync("MATCH (n:Schools {SchoolID: $id}) RETURN n", new { id });
            var records = await cursor.ToListAsync();

            if (records.Count == 0) return null;

            var node = records[0]["n"].As<INode>();
            return new SchoolsModel
            {
                SchoolID = node.Properties["SchoolID"].As<int>(),
                SchoolName = node.Properties["SchoolName"].As<string>()
            };
        }

        public async Task<bool> InsertAsync(SchoolsModel dto)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            int idNum = await GetNumberOfPeople() + 1;

            var cursor = await session.RunAsync(@"
                CREATE (person:Schools { SchoolID: $id, SchoolName: $name })
                RETURN person",
                new { id = idNum, name = dto.SchoolName }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<bool> UpdateAsync(int id, SchoolsModel dto)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (n:Schools {SchoolID: $id})
                SET n.SchoolName = $name
                RETURN n",
                new { id, name = dto.SchoolName}
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (p:Schools {SchoolID: $id})
                DETACH DELETE p
                RETURN p",
                new { id }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<int> GetNumberOfPeople()
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var cursor = await session.RunAsync("MATCH (n:Schools) RETURN n");
            var records = await cursor.ToListAsync();
            return records.Count;
        }
    }
}

using Neo4j.Driver;
using Stalkr.Models;

namespace Stalkr.Repositories
{
    public class HobbyRepository : IRepository<HobbyModel>
    {
        private readonly IDriver _driver;

        public HobbyRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (p:Hobbies {HobbyID: $id})
                DETACH DELETE p
                RETURN p",
                new { id }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<HobbyModel?> FindByIdAsync(int id)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var cursor = await session.RunAsync("MATCH (n:Hobbies {HobbyID: $id}) RETURN n", new { id });
            var records = await cursor.ToListAsync();

            if (records.Count == 0) return null;

            var node = records[0]["n"].As<INode>();
            return new HobbyModel
            {
                HobbyID = node.Properties["HobbyID"].As<int>(),
                HobbyName = node.Properties["HobbyName"].As<string>()
            };
        }

        public async Task<IEnumerable<HobbyModel>> GetAllAsync()
        {
            var hobbies = new List<HobbyModel>();

            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var cursor = await session.RunAsync("MATCH (n:Hobbies) RETURN n");
            var records = await cursor.ToListAsync();

            foreach (var record in records)
            {
                var node = record["n"].As<INode>();
                var hobby = new HobbyModel
                {
                    HobbyID = node.Properties["HobbyID"].As<int>(),
                    HobbyName = node.Properties["HobbyName"].As<string>()
                };
                hobbies.Add(hobby);
            }

            return hobbies;
        }

        public async Task<bool> InsertAsync(HobbyModel dto)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            int idNum = await GetNumberOfHobbies() + 1;

            var cursor = await session.RunAsync(@"
                CREATE (person:Hobbies { HobbyID: $id, HobbyName: $hobbyName})
                RETURN person",
                new { id = idNum, hobbyName = dto.HobbyName }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<bool> UpdateAsync(int id, HobbyModel dto)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (n:Hobbies {HobbyID: $id})
                SET n.HobbyName = $hobyName
                RETURN n",
                new { id, hobyName = dto.HobbyName }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<int> GetNumberOfHobbies()
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var cursor = await session.RunAsync("MATCH (n:Hobbies) RETURN n");
            var records = await cursor.ToListAsync();
            return records.Count;
        }
    }
}

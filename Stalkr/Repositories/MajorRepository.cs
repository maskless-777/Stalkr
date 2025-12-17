using Neo4j.Driver;
using Stalkr.Models;

namespace Stalkr.Repositories
{
    public class MajorRepository : IRepository<MajorModel>
    {
        private readonly IDriver _driver;

        public MajorRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<IEnumerable<MajorModel>> GetAllAsync()
        {
            var majors = new List<MajorModel>();

            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var cursor = await session.RunAsync("MATCH (m:Majors) RETURN m");
            var records = await cursor.ToListAsync();

            foreach (var record in records)
            {
                var node = record["m"].As<INode>();
                majors.Add(new MajorModel
                {
                    MajorID = node.Properties["MajorID"].As<int>(),
                    MajorName = node.Properties["MajorName"].As<string>()
                });
            }

            return majors;
        }

        public async Task<MajorModel?> FindByIdAsync(int id)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var cursor = await session.RunAsync(
                "MATCH (m:Majors {MajorID: $id}) RETURN m",
                new { id }
            );

            var records = await cursor.ToListAsync();
            if (records.Count == 0) return null;

            var node = records[0]["m"].As<INode>();
            return new MajorModel
            {
                MajorID = node.Properties["MajorID"].As<int>(),
                MajorName = node.Properties["MajorName"].As<string>()
            };
        }

        public async Task<bool> InsertAsync(MajorModel dto)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            int idNum = (await GetNumberOfMajors()) + 1;

            var cursor = await session.RunAsync(
                "CREATE (m:Majors { MajorID: $id, MajorName: $name }) RETURN m",
                new { id = idNum, name = dto.MajorName }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<bool> UpdateAsync(int id, MajorModel dto)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(
                "MATCH (m:Majors {MajorID: $id}) SET m.MajorName = $name RETURN m",
                new { id, name = dto.MajorName }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(
                "MATCH (m:Majors {MajorID: $id}) DETACH DELETE m RETURN m",
                new { id }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<int> GetNumberOfMajors()
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var cursor = await session.RunAsync("MATCH (m:Majors) RETURN m");
            var records = await cursor.ToListAsync();
            return records.Count;
        }
    }
}

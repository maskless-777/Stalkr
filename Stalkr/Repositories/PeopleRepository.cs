using System;
using Neo4j.Driver;
using Stalkr.Models;

namespace Stalkr.Repositories
{
    public class PeopleRepository : IRepository<PeopleModel>
    {
        private readonly IDriver _driver;

        public PeopleRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<IEnumerable<PeopleModel>> GetAllAsync()
        {
            var people = new List<PeopleModel>();

            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var cursor = await session.RunAsync("MATCH (n:People) RETURN n");
            var records = await cursor.ToListAsync();

            foreach (var record in records)
            {
                var node = record["n"].As<INode>();
                var person = new PeopleModel
                {
                    PersonID = node.Properties["PersonID"].As<int>(),
                    FirstName = node.Properties["FirstName"].As<string>(),
                    LastName = node.Properties["LastName"].As<string>(),
                    Age = node.Properties["Age"].As<int>()
                };
                people.Add(person);
            }

            return people;
        }

        public async Task<PeopleModel?> FindByIdAsync(int id)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var cursor = await session.RunAsync("MATCH (n:People {PersonID: $id}) RETURN n", new { id });
            var records = await cursor.ToListAsync();

            if (records.Count == 0) return null;

            var node = records[0]["n"].As<INode>();
            return new PeopleModel
            {
                PersonID = node.Properties["PersonID"].As<int>(),
                FirstName = node.Properties["FirstName"].As<string>(),
                LastName = node.Properties["LastName"].As<string>(),
                Age = node.Properties["Age"].As<int>()
            };
        }

        public async Task<bool> InsertAsync(PeopleModel dto)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            int idNum = await GetNumberOfPeople() + 1;

            var cursor = await session.RunAsync(@"
                CREATE (person:People { PersonID: $id, FirstName: $firstname, LastName: $lastname, Age: $age })
                RETURN person",
                new { id = idNum, firstname = dto.FirstName, lastname = dto.LastName, age = dto.Age }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<bool> UpdateAsync(int id, PeopleModel dto)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (n:People {PersonID: $id})
                SET n.FirstName = $firstname, n.LastName = $lastname, n.Age = $age
                RETURN n",
                new { id, firstname = dto.FirstName, lastname = dto.LastName, age = dto.Age }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (p:People {PersonID: $id})
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
            var cursor = await session.RunAsync("MATCH (n:People) RETURN n");
            var records = await cursor.ToListAsync();
            return records.Count;
        }
    }
}

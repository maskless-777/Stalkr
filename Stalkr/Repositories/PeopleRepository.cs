
using Neo4j.Driver;
using Stalkr.Models;

namespace Stalkr.Repositories
{
    public class PeopleRepository : IRepository<PeopleModel>
    {
        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PeopleModel?> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PeopleModel>> GetAllAsync()
        {
            const string dbUri = "<dbUri>";
            const string dbUser = "<dbUser>";
            const string dbPassword = "<dbPass>";
            Console.WriteLine("in GetAllAsync");
            var people = new List<PeopleModel>();

            await using var driver = GraphDatabase.Driver(dbUri, AuthTokens.Basic(dbUser, dbPassword));
            await driver.VerifyConnectivityAsync();

            var result = await driver.ExecutableQuery(@"
                MATCH (n:People)
                ")
                .WithConfig(new QueryConfig(database: "neo4j"))
                .ExecuteAsync();

            Console.WriteLine(result);
            Console.WriteLine("Fetched records:");

            foreach (var record in result.Result)
            {
                var person = new PeopleModel
                {
                    PersonID = record["n"].As<INode>().Properties["PersonID"].As<int>(),
                    FirstName = record["n"].As<INode>().Properties["FirstName"].As<string>(),
                    LastName = record["n"].As<INode>().Properties["LastName"].As<string>(),
                    Age = record["n"].As<INode>().Properties["Age"].As<int>()
                };

                Console.WriteLine("person: " + person);
                people.Add(person);
            }

            return people;
        }

        public Task<bool> InsertAsync(PeopleModel dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(int id, PeopleModel dto)
        {
            throw new NotImplementedException();
        }
    }
}

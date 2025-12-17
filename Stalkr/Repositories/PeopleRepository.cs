
using System;
using Neo4j.Driver;
using Stalkr.Models;

namespace Stalkr.Repositories
{
    public class PeopleRepository : IRepository<PeopleModel>
    {


        const string dbUri = "<uri>";
        const string dbUser = "<user>";
        const string dbPassword = "<pass>";

        public async Task<bool> DeleteAsync(int id)
        {
            Console.WriteLine("in deteeAsync");

            await using var driver = GraphDatabase.Driver(dbUri, AuthTokens.Basic(dbUser, dbPassword));
            await driver.VerifyConnectivityAsync();

            var result = await driver.ExecutableQuery(@"
                MATCH (p:People {PersonID: $id})
                    DETACH DELETE p
                ")
                .WithConfig(new QueryConfig(database: "neo4j"))
                .WithParameters(new { id })
                .ExecuteAsync();

            Console.WriteLine("after result delete");

            return result.Result.Count > 0;
        }

        public async Task<PeopleModel?> FindByIdAsync(int id)
        {
            PeopleModel? person = null;

            await using var driver = GraphDatabase.Driver(dbUri, AuthTokens.Basic(dbUser, dbPassword));
            await driver.VerifyConnectivityAsync();

            var result = await driver.ExecutableQuery(@"
                MATCH (n:People {PersonID: $id}) RETURN n;
                ")
                .WithConfig(new QueryConfig(database: "neo4j"))
                .WithParameters(new { id })
                .ExecuteAsync();


            if (result.Result.Count == 0)
            {
                return null;
            }

            foreach (var record in result.Result)
            {
                person = new PeopleModel
                {
                    PersonID = record["n"].As<INode>().Properties["PersonID"].As<int>(),
                    FirstName = record["n"].As<INode>().Properties["FirstName"].As<string>(),
                    LastName = record["n"].As<INode>().Properties["LastName"].As<string>(),
                    Age = record["n"].As<INode>().Properties["Age"].As<int>()
                };

                Console.WriteLine("person: " + person);
            }

            return person;
        }

        public async Task<IEnumerable<PeopleModel>> GetAllAsync()
        {
            var people = new List<PeopleModel>();

            await using var driver = GraphDatabase.Driver(dbUri, AuthTokens.Basic(dbUser, dbPassword));
            await driver.VerifyConnectivityAsync();

            var result = await driver.ExecutableQuery(@"
                MATCH (n:People) RETURN n
                ")
                .WithConfig(new QueryConfig(database: "neo4j"))
                .ExecuteAsync();

            foreach (var record in result.Result)
            {
                var person = new PeopleModel
                {
                    PersonID = record["n"].As<INode>().Properties["PersonID"].As<int>(),
                    FirstName = record["n"].As<INode>().Properties["FirstName"].As<string>(),
                    LastName = record["n"].As<INode>().Properties["LastName"].As<string>(),
                    Age = record["n"].As<INode>().Properties["Age"].As<int>()
                };

                //Console.WriteLine("person: " + person);
                people.Add(person);
            }

            return people;
        }

        public async Task<bool> InsertAsync(PeopleModel dto)
        {

            await using var driver = GraphDatabase.Driver(dbUri, AuthTokens.Basic(dbUser, dbPassword));
            await driver.VerifyConnectivityAsync();

            int idNum = await GetNumberOfPeople() + 1;


            var result = await driver.ExecutableQuery(@"
                CREATE(person: People { PersonID: $id, FirstName: $firstname, LastName: $lastname, Age: $age})
                    RETURN person;
                ")
                .WithConfig(new QueryConfig(database: "neo4j"))
                .WithParameters(new { firstname = dto.FirstName, lastname = dto.LastName, age = dto.Age, id = idNum })
                .ExecuteAsync();

            return result.Result.Count > 0;
        }

        public async Task<bool> UpdateAsync(int id, PeopleModel dto)
        {
            await using var driver = GraphDatabase.Driver(dbUri, AuthTokens.Basic(dbUser, dbPassword));
            await driver.VerifyConnectivityAsync();

            var result = await driver.ExecutableQuery(@"
                MATCH (n:People {PersonID: $id})
                    SET n.FirstName = $firstname, n.LastName = $lastname, n.Age = $age;
                ")
                .WithConfig(new QueryConfig(database: "neo4j"))
                .WithParameters(new { firstname = dto.FirstName, lastname = dto.LastName, age = dto.Age, id = id })
                .ExecuteAsync();

            return result.Result.Count > 0;
        }

        public async Task<int> GetNumberOfPeople()
        {

            var people = new List<PeopleModel>();

            await using var driver = GraphDatabase.Driver(dbUri, AuthTokens.Basic(dbUser, dbPassword));
            await driver.VerifyConnectivityAsync();

            var result = await driver.ExecutableQuery(@"
                MATCH (n:People) RETURN n
                ")
                .WithConfig(new QueryConfig(database: "neo4j"))
                .ExecuteAsync();

            return result.Result.Count;
        }
    }
}

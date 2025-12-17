using Neo4j.Driver;
using Stalkr.Models;

namespace Stalkr.Repositories
{
    public class EnrolledInRepository
    {
        private readonly IDriver _driver;

        public EnrolledInRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<bool> CreateAsync(EnrolledInRelationshipModel rel)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (p:People {PersonID: $personId}), (c:Classes {CourseID: $courseId})
                MERGE (p)-[r:ENROLLED_IN]->(c)
                RETURN r",
                new { personId = rel.PersonID, courseId = rel.CourseID }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<bool> DeleteAsync(int personId, int courseId)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (p:People {PersonID: $personId})-[r:ENROLLED_IN]->(c:Classes {CourseID: $courseId})
                DELETE r
                RETURN r",
                new { personId, courseId }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }
        public async Task<IEnumerable<EnrolledInRelationshipModel>> GetAllWithDetailsAsync()
        {
            var list = new List<EnrolledInRelationshipModel>();
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (p:People)-[r:ENROLLED_IN]->(c:Classes)
                RETURN p, c"
            );

            var records = await cursor.ToListAsync();
            foreach (var record in records)
            {
                var personNode = record["p"].As<INode>();
                var classNode = record["c"].As<INode>();

                list.Add(new EnrolledInRelationshipModel
                {
                    PersonID = personNode.Properties["PersonID"].As<int>(),
                    CourseID = classNode.Properties["CourseID"].As<string>(),
                    Person = new PeopleModel
                    {
                        PersonID = personNode.Properties["PersonID"].As<int>(),
                        FirstName = personNode.Properties["FirstName"].As<string>(),
                        LastName = personNode.Properties["LastName"].As<string>(),
                        Age = personNode.Properties["Age"].As<int>()
                    },
                    Class = new ClassesModel
                    {
                        CourseID = classNode.Properties["CourseID"].As<string>(),
                        CourseName = classNode.Properties["CourseName"].As<string>(),
                        CourseTerm = classNode.Properties["CourseTerm"].As<string>()
                    }
                });
            }

            return list;
        }

        public async Task<IEnumerable<EnrolledInRelationshipModel>> GetByPersonIdAsync(int personId)
        {
            var list = new List<EnrolledInRelationshipModel>();
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (p:People {PersonID: $personId})-[r:ENROLLED_IN]->(c:Classes)
                RETURN p, c",
                new { personId }
            );

            var records = await cursor.ToListAsync();
            foreach (var record in records)
            {
                var personNode = record["p"].As<INode>();
                var classNode = record["c"].As<INode>();

                list.Add(new EnrolledInRelationshipModel
                {
                    PersonID = personNode.Properties["PersonID"].As<int>(),
                    CourseID = classNode.Properties["CourseID"].As<string>(),
                    Person = new PeopleModel
                    {
                        PersonID = personNode.Properties["PersonID"].As<int>(),
                        FirstName = personNode.Properties["FirstName"].As<string>(),
                        LastName = personNode.Properties["LastName"].As<string>(),
                        Age = personNode.Properties["Age"].As<int>()
                    },
                    Class = new ClassesModel
                    {
                        CourseID = classNode.Properties["CourseID"].As<string>(),
                        CourseName = classNode.Properties["CourseName"].As<string>(),
                        CourseTerm = classNode.Properties["CourseTerm"].As<string>()
                    }
                });
            }

            return list;
        }
    }
}

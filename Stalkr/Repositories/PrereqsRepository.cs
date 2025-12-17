using Neo4j.Driver;
using Stalkr.Models;

namespace Stalkr.Repositories
{
    public class PrereqRepository
    {
        private readonly IDriver _driver;

        public PrereqRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<bool> CreateAsync(PrereqRelationshipModel rel)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (from:Classes {CourseID: $fromId}), (to:Classes {CourseID: $toId})
                MERGE (from)-[r:PREREQ]->(to)
                RETURN r",
                new { fromId = rel.FromCourseID, toId = rel.ToCourseID }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<bool> DeleteAsync(string fromCourseId, string toCourseId)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (from:Classes {CourseID: $fromId})-[r:PREREQ]->(to:Classes {CourseID: $toId})
                DELETE r
                RETURN r",
                new { fromId = fromCourseId, toId = toCourseId }
            );

            var records = await cursor.ToListAsync();
            return records.Count > 0;
        }

        public async Task<IEnumerable<PrereqRelationshipModel>> GetAllAsync()
        {
            var list = new List<PrereqRelationshipModel>();
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (from:Classes)-[r:PREREQ]->(to:Classes)
                RETURN from, to"
            );

            var records = await cursor.ToListAsync();
            foreach (var record in records)
            {
                var fromNode = record["from"].As<INode>();
                var toNode = record["to"].As<INode>();

                list.Add(new PrereqRelationshipModel
                {
                    FromCourseID = fromNode.Properties["CourseID"].As<string>(),
                    ToCourseID = toNode.Properties["CourseID"].As<string>(),
                    From = new ClassesModel
                    {
                        CourseID = fromNode.Properties["CourseID"].As<string>(),
                        CourseName = fromNode.Properties["CourseName"].As<string>(),
                        CourseTerm = fromNode.Properties["CourseTerm"].As<string>()
                    },
                    To = new ClassesModel
                    {
                        CourseID = toNode.Properties["CourseID"].As<string>(),
                        CourseName = toNode.Properties["CourseName"].As<string>(),
                        CourseTerm = toNode.Properties["CourseTerm"].As<string>()
                    }
                });
            }

            return list;
        }

        // Get all prerequisites for a specific course
        public async Task<IEnumerable<PrereqRelationshipModel>> GetByToCourseIdAsync(string courseId)
        {
            var list = new List<PrereqRelationshipModel>();
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var cursor = await session.RunAsync(@"
                MATCH (from:Classes)-[r:PREREQ]->(to:Classes {CourseID: $toId})
                RETURN from, to",
                new { toId = courseId }
            );

            var records = await cursor.ToListAsync();
            foreach (var record in records)
            {
                var fromNode = record["from"].As<INode>();
                var toNode = record["to"].As<INode>();

                list.Add(new PrereqRelationshipModel
                {
                    FromCourseID = fromNode.Properties["CourseID"].As<string>(),
                    ToCourseID = toNode.Properties["CourseID"].As<string>(),
                    From = new ClassesModel
                    {
                        CourseID = fromNode.Properties["CourseID"].As<string>(),
                        CourseName = fromNode.Properties["CourseName"].As<string>(),
                        CourseTerm = fromNode.Properties["CourseTerm"].As<string>()
                    },
                    To = new ClassesModel
                    {
                        CourseID = toNode.Properties["CourseID"].As<string>(),
                        CourseName = toNode.Properties["CourseName"].As<string>(),
                        CourseTerm = toNode.Properties["CourseTerm"].As<string>()
                    }
                });
            }

            return list;
        }
    }
}

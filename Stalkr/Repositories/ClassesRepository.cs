using Neo4j.Driver;
using Stalkr.Models;

namespace Stalkr.Repositories
{
    public class ClassesRepository : IRepository<ClassesModel>
    {
        private readonly IDriver _driver;

        public ClassesRepository(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<IEnumerable<ClassesModel>> GetAllAsync()
        {
            var list = new List<ClassesModel>();
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var result = await session.RunAsync(@"
                MATCH (c:Classes)
                RETURN c
            ");

            await result.ForEachAsync(record =>
            {
                var node = record["c"].As<INode>();
                list.Add(new ClassesModel
                {
                    CourseID = node.Properties["CourseID"].As<string>(),
                    CourseName = node.Properties["CourseName"].As<string>(),
                    CourseTerm = node.Properties["CourseTerm"].As<string>()
                });
            });

            return list;
        }

        public async Task<ClassesModel?> FindByIdAsync(string id)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            var result = await session.RunAsync(@"
                MATCH (c:Classes { CourseID: $id })
                RETURN c
                LIMIT 1
            ", new { id });

            var record = await result.FetchAsync() ? result.Current : null;

            if (record == null)
                return null;

            var node = record["c"].As<INode>();

            return new ClassesModel
            {
                CourseID = node.Properties["CourseID"].As<string>(),
                CourseName = node.Properties["CourseName"].As<string>(),
                CourseTerm = node.Properties["CourseTerm"].As<string>()
            };
        }

        public async Task<bool> InsertAsync(ClassesModel dto)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            await session.RunAsync(@"
                CREATE (c:Classes {
                    CourseID: $CourseID,
                    CourseName: $CourseName,
                    CourseTerm: $CourseTerm
                })
            ", dto);

            return true;
        }

        public async Task<bool> UpdateAsync(string id, ClassesModel dto)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            await session.RunAsync(@"
                MATCH (c:Classes { CourseID: $id })
                SET c.CourseName = $CourseName,
                    c.CourseTerm = $CourseTerm
            ", new { id, dto.CourseName, dto.CourseTerm });

            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase("neo4j"));

            await session.RunAsync(@"
                MATCH (c:Classes { CourseID: $id })
                DETACH DELETE c
            ", new { id });

            return true;
        }

        #region IRepository<int> compatibility (not used, optional)
        public Task<ClassesModel?> FindByIdAsync(int id) => throw new NotImplementedException();
        public Task<bool> UpdateAsync(int id, ClassesModel dto) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(int id) => throw new NotImplementedException();
        #endregion
    }
}

using Nightingale.Entities;
using Nightingale.Sessions;
using Xunit;

namespace Nightingale.Tests
{
    public class RepositoryTests
    {
        public RepositoryTests()
        {
            DependencyResolver.Clear();
        }

        [Fact]
        public void GetById_Calls_Session_Load()
        {
            // arrange
            var sessionMock = TestHelper.SetupMock<ISession>();
            sessionMock.Setup(s => s.Load(1, typeof(Entity))).Returns((Entity)null);

            var repository = new Repository(sessionMock.Object);

            // act
            var result = repository.GetById<Entity>(1);

            // assert
            Assert.Null(result);

            sessionMock.VerifyAll();
        }

        [Fact]
        public void GetByIdAndType_Calls_Session_Load()
        {
            // arrange
            var sessionMock = TestHelper.SetupMock<ISession>();
            sessionMock.Setup(s => s.Load(1, typeof(Entity))).Returns((Entity)null);

            var repository = new Repository(sessionMock.Object);

            // act
            var result = repository.GetByIdAndType(1, typeof(Entity));

            // assert
            Assert.Null(result);

            sessionMock.VerifyAll();
        }

        [Fact]
        public void ExecuteFunc_Calls_Session()
        {
            // arrange
            var sessionMock = TestHelper.SetupMock<ISession>();
            var repository = new Repository(sessionMock.Object);

            sessionMock.Setup(s => s.ExecuteFunc<int>("dbo.test", null)).Returns(0);

            // act
            var result = repository.ExecuteFunc<int>("dbo.test", null);

            // assert
            Assert.Equal(0, result);

            sessionMock.VerifyAll();
        }
    }
}

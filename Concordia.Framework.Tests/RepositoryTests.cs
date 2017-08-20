using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Concordia.Framework.Entities;
using Concordia.Framework.Queries;
using Concordia.Framework.Sessions;
using Moq;
using Xunit;

namespace Concordia.Framework.Tests
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

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void GetList_Adds_Deleted_Check_To_Query(bool withExpression)
        {
            // arrange
            var sessionMock = TestHelper.SetupMock<ISession>();

            IQuery queryCallback = null;

            sessionMock.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>((queryObj) => queryCallback = queryObj);

            var repository = new Repository(sessionMock.Object);

            // act
            var result = repository.GetList(withExpression ? x => x.Id == 1 : (Expression<Func<Entity, bool>>)null);

            // assert
            var query = queryCallback as Query;
            var conditionItem = query.ConditionGroups.First().Conditions.FirstOrDefault(x => x.PropertyPath == "Deleted");
            Assert.NotNull(conditionItem);
            Assert.NotNull(result);
            Assert.Equal("Deleted", conditionItem.PropertyPath);
            Assert.False((bool)conditionItem.EquationValue);

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Concordia.Framework.Entities;
using Concordia.Framework.Queries;
using Concordia.Framework.Tests.DataSources;

namespace Concordia.Framework.Tests
{
    [TestFixture]
    public class RepositoryTests
    {
        [SetUp]
        public void Setup()
        {
            DependencyResolver.Clear();
        }

        [Test]
        public void GetById_Calls_Session_Load()
        {
            // arrange
            var sessionMock = TestHelper.CreateSessionMock();
            sessionMock.Setup(s => s.Load(1, typeof(Entity))).Returns((Entity)null);

            var repository = new Repository(sessionMock.Object);

            // act
            var result = repository.GetById<Entity>(1);

            // assert
            Assert.That(result, Is.Null);

            sessionMock.VerifyAll();
        }

        [Test]
        public void GetByIdAndType_Calls_Session_Load()
        {
            // arrange
            var sessionMock = TestHelper.CreateSessionMock();
            sessionMock.Setup(s => s.Load(1, typeof(Entity))).Returns((Entity)null);

            var repository = new Repository(sessionMock.Object);

            // act
            var result = repository.GetByIdAndType(1, typeof(Entity));

            // assert
            Assert.That(result, Is.Null);

            sessionMock.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetList_Adds_Deleted_Check_To_Query(bool withExpression)
        {
            // arrange
            var sessionMock = TestHelper.CreateSessionMock();

            IQuery queryCallback = null;

            sessionMock.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>((queryObj) => queryCallback = queryObj);

            var repository = new Repository(sessionMock.Object);

            // act
            var result = repository.GetList<Entity>(withExpression ? x => x.Id == 1 : (Expression<Func<Entity, bool>>)null);

            // assert
            var query = queryCallback as Query;
            var conditionItem = query.ConditionGroups.First().Conditions.FirstOrDefault(x => x.PropertyPath == "Deleted");
            Assert.That(conditionItem, Is.Not.Null);
            Assert.That(result, Is.Not.Null);
            Assert.That(conditionItem.PropertyPath, Is.EqualTo("Deleted"));
            Assert.That(conditionItem.EquationValue, Is.False);

            sessionMock.VerifyAll();
        }

        [Test]
        public void ExecuteFunc_Calls_Session()
        {
            // arrange
            var sessionMock = TestHelper.CreateSessionMock();
            var repository = new Repository(sessionMock.Object);

            sessionMock.Setup(s => s.ExecuteFunc<int>("dbo.test", null)).Returns(0);

            // act
            var result = repository.ExecuteFunc<int>("dbo.test", null);

            // assert
            Assert.That(result, Is.EqualTo(0));
            sessionMock.VerifyAll();
        }
    }
}

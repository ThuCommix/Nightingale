using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using ThuCommix.EntityFramework.Entities;
using ThuCommix.EntityFramework.Queries;

namespace ThuCommix.EntityFramework.Tests
{
    [TestFixture]
    public class RepositoryTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Repository_EntityListeners_Initialized()
        {
            // arrange
            var sessionMock = TestHelper.CreateSessionMock();

            // act
            var repository = new Repository(sessionMock.Object);

            // assert
            Assert.That(repository.EntityListeners, Is.Not.Null);

            sessionMock.VerifyAll();
        }

        [Test]
        public void Save_Calls_EntityListeners()
        {
            // arrange
            var sessionMock = TestHelper.CreateSessionMock();
            var entityListener = TestHelper.SetupMock<IEntityListener>();

            entityListener.Setup(s => s.Save(null)).Returns(true);
            sessionMock.Setup(s => s.SaveOrUpdate(null));

            var repository = new Repository(sessionMock.Object);
            repository.EntityListeners.Add(entityListener.Object);

            // act
            repository.Save(null);

            // assert
            sessionMock.VerifyAll();
            entityListener.VerifyAll();
        }

        [Test]
        public void Save_Throws_Exception_If_EntityListener_Returns_False()
        {
            // arrange
            var sessionMock = TestHelper.CreateSessionMock();
            var entityListener = TestHelper.SetupMock<IEntityListener>();

            entityListener.Setup(s => s.Save(null)).Returns(false);

            var repository = new Repository(sessionMock.Object);
            repository.EntityListeners.Add(entityListener.Object);

            // act
            Assert.Throws<InvalidOperationException>(delegate { repository.Save(null); });

            // assert
            sessionMock.VerifyAll();
            entityListener.VerifyAll();
        }

        [Test]
        public void Delete_Calls_EntityListeners()
        {
            // arrange
            var sessionMock = TestHelper.CreateSessionMock();
            var entityListener = TestHelper.SetupMock<IEntityListener>();

            entityListener.Setup(s => s.Delete(null)).Returns(true);
            sessionMock.Setup(s => s.Delete(null));

            var repository = new Repository(sessionMock.Object);
            repository.EntityListeners.Add(entityListener.Object);

            // act
            repository.Delete(null);

            // assert
            sessionMock.VerifyAll();
            entityListener.VerifyAll();
        }

        [Test]
        public void Delete_Throws_Exception_If_EntityListener_Returns_False()
        {
            // arrange
            var sessionMock = TestHelper.CreateSessionMock();
            var entityListener = TestHelper.SetupMock<IEntityListener>();

            entityListener.Setup(s => s.Delete(null)).Returns(false);

            var repository = new Repository(sessionMock.Object);
            repository.EntityListeners.Add(entityListener.Object);

            // act
            Assert.Throws<InvalidOperationException>(delegate { repository.Delete(null); });

            // assert
            sessionMock.VerifyAll();
            entityListener.VerifyAll();
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
        public void Commit_Calls_CommitListeners()
        {
            // arrange
            var sessionMock = TestHelper.CreateSessionMock();
            var repository = new Repository(sessionMock.Object);

            var commitListener = TestHelper.SetupMock<ICommitListener>();
            bool result = false;
            commitListener.Setup(s => s.Commit()).Callback(() => result = true);
            sessionMock.Setup(s => s.Commit());

            repository.CommitListeners.Add(commitListener.Object);

            // act
            repository.Commit();

            // assert
            Assert.That(result, Is.True);

            commitListener.VerifyAll();
            sessionMock.VerifyAll();
        }
    }
}

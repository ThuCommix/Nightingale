using System.Collections.Generic;
using NUnit.Framework;
using Concordia.Framework.Entities;
using Concordia.Framework.Sessions;
using Concordia.Framework.Tests.DataSources;

namespace Concordia.Framework.Tests.Sessions
{
    [TestFixture]
    public class StatefulSessionTests
    {
        [SetUp]
        public void Setup()
        {
            DependencyResolver.Clear();
        }

        [Test]
        public void SaveOrUpdate_Prevent_Saving_Of_Evicted_Entity()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new StatefulSession(dataProviderMock.Object);
            var entity = TestHelper.CreateEntityWithId<Artist>(1);

            TestHelper.SetEntityEvict(entity, true);

            // act
            Assert.Throws<SessionException>(() => session.SaveOrUpdate(entity));

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Flush_Check_For_Evicted_Entities_In_FlushList()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new StatefulSession(dataProviderMock.Object);
            var entity = TestHelper.CreateEntityWithId<Artist>(1);

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.Save)).Returns(new List<Entity> { entity });

            session.SaveOrUpdate(entity);
            TestHelper.SetEntityEvict(entity, true);

            // act
            Assert.Throws<SessionException>(() => session.Flush());

            // assert
            dataProviderMock.VerifyAll();
            entityServiceMock.VerifyAll();
        }

        [Test]
        public void Load_Query_PersistenceCache_First()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new StatefulSession(dataProviderMock.Object);
            var entity = TestHelper.CreateEntityWithId<Artist>(1);

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.Save)).Returns(new List<Entity> { entity });

            session.SaveOrUpdate(entity);

            // act
            var result = session.Load(1, typeof(Artist));

            // assert
            Assert.That(result, Is.EqualTo(entity));

            entityServiceMock.VerifyAll();
            dataProviderMock.VerifyAll();
        }
    }
}

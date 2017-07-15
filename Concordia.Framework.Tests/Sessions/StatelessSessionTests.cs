using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Concordia.Framework.Entities;
using Concordia.Framework.Queries;
using Concordia.Framework.Sessions;
using Concordia.Framework.Tests.DataSources;

namespace Concordia.Framework.Tests.Sessions
{
    [TestFixture]
    public class StatelessSessionTests
    {
        [SetUp]
        public void Setup()
        {
            DependencyResolver.Clear();
        }

        [Test]
        public void Evict_Does_Nothing()
        {
            // arrange
            var connectionMock = TestHelper.SetupConnection();
            var session = new StatelessSession(connectionMock.Object);
            var entity = TestHelper.CreateEntityWithId<Artist>(1);

            // act
            session.Evict(entity);

            // assert
            Assert.That(TestHelper.CheckEvicted(entity), Is.False);

            connectionMock.VerifyAll();
        }

        [TestCase(SessionFlushMode.Manual)]
        [TestCase(SessionFlushMode.Always)]
        [TestCase(SessionFlushMode.Commit)]
        [TestCase(SessionFlushMode.Intelligent)]
        public void SaveOrUpdate_Flushes_On_Whichever_FlushMode(SessionFlushMode flushMode)
        {
            // arrange
            var connectionMock = TestHelper.SetupConnection();
            var session = new StatelessSession(connectionMock.Object);
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            entity.Name = "Test";

            entity.PropertyChangeTracker.AddPropertyChangedItem("Name", null, entity.Name);

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.Save)).Returns(new List<Entity> { entity });
            entityServiceMock.Setup(s => s.UpdateForeignFields(entity));

            connectionMock.Setup(s => s.ExecuteNonQuery(It.IsAny<IQuery>())).Returns(1);

            TestHelper.SetupEntityMetadataServices();

            // act
            session.SaveOrUpdate(entity);

            // assert
            connectionMock.VerifyAll();
            entityServiceMock.VerifyAll();
        }
    }
}

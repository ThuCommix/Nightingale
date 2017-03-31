using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using ThuCommix.EntityFramework.Entities;
using ThuCommix.EntityFramework.Queries;
using ThuCommix.EntityFramework.Sessions;
using ThuCommix.EntityFramework.Tests.DataSources;

namespace ThuCommix.EntityFramework.Tests.Sessions
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
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new StatelessSession(dataProviderMock.Object);
            var entity = TestHelper.CreateEntityWithId<Artist>(1);

            // act
            session.Evict(entity);

            // assert
            Assert.That(TestHelper.CheckEvicted(entity), Is.False);

            dataProviderMock.VerifyAll();
        }

        [TestCase(SessionFlushMode.Manual)]
        [TestCase(SessionFlushMode.Always)]
        [TestCase(SessionFlushMode.Commit)]
        [TestCase(SessionFlushMode.Intelligent)]
        public void SaveOrUpdate_Flushes_On_Whichever_FlushMode(SessionFlushMode flushMode)
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new StatelessSession(dataProviderMock.Object);
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            entity.Name = "Test";

            entity.PropertyChangeTracker.AddPropertyChangedItem("Name", null, entity.Name);

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.Save)).Returns(new List<Entity> { entity });
            entityServiceMock.Setup(s => s.UpdateForeignFields(entity));

            dataProviderMock.Setup(s => s.ExecuteNonQuery(It.IsAny<IQuery>())).Returns(1);

            TestHelper.SetupEntityMetadataServices();

            // act
            session.SaveOrUpdate(entity);

            // assert
            dataProviderMock.VerifyAll();
            entityServiceMock.VerifyAll();
        }
    }
}

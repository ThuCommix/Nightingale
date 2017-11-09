using System.Collections.Generic;
using Moq;
using Nightingale.Entities;
using Nightingale.Queries;
using Nightingale.Sessions;
using Nightingale.Tests.DataSources;
using Xunit;

namespace Nightingale.Tests.Sessions
{
    public class StatelessSessionTests
    {
        public StatelessSessionTests()
        {
            DependencyResolver.Clear();
        }

        [Fact]
        public void Evict_Does_Nothing()
        {
            // arrange
            var connectionMock = TestHelper.SetupConnection();
            var session = new StatelessSession(connectionMock.Object);
            var entity = TestHelper.CreateEntityWithId<Artist>(1);

            // act
            session.Evict(entity);

            // assert
            Assert.False(TestHelper.CheckEvicted(entity));

            connectionMock.VerifyAll();
        }

        [Theory]
        [InlineData(FlushMode.Manual)]
        [InlineData(FlushMode.Always)]
        [InlineData(FlushMode.Commit)]
        [InlineData(FlushMode.Intelligent)]
        public void SaveOrUpdate_Flushes_On_Whichever_FlushMode(FlushMode flushMode)
        {
            // arrange
            var connectionMock = TestHelper.SetupConnection();
            var session = new StatelessSession(connectionMock.Object) { FlushMode = flushMode };
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

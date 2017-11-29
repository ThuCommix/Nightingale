using System;
using System.Collections.Generic;
using System.Reflection;
using Nightingale.Entities;
using Nightingale.Sessions;
using Nightingale.Tests.DataSources;
using Xunit;

namespace Nightingale.Tests.Sessions
{
    public class SessionTests
    {
        public SessionTests()
        {
            DependencyResolver.Clear();
        }

        [Fact]
        public void Session_Ctor_Checks_Connection_For_Null()
        {
            // act
            Assert.Throws<ArgumentNullException>(() => new Session(null));
        }

        [Fact]
        public void Delete_Throws_Exception_When_Entity_Is_Null()
        {
            // arrange
            var connectionMock = TestHelper.SetupMock<IConnection>();
            var session = new Session(connectionMock.Object);

            // act
            Assert.Throws<ArgumentNullException>(() => session.Delete(null));

            // assert
            connectionMock.VerifyAll();
        }

        [Fact]
        public void Delete_Return_When_DeletionMode_None()
        {
            // arrange
            var connectionMock = TestHelper.SetupMock<IConnection>();
            var session = new Session(connectionMock.Object) {DeletionBehavior = DeletionBehavior.None};

            // act
            session.Delete(new Artist());

            // assert
            connectionMock.VerifyAll();
        }

        [Fact]
        public void Delete_Marks_All_Entities_From_EntityService_As_Deleted()
        {
            // arrange
            var connectionMock = TestHelper.SetupMock<IConnection>();
            var session = new Session(connectionMock.Object);
            var entity = new Artist();
            var entity2 = new Artist();

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.SaveDelete)).Returns(new List<Entity> {entity, entity2});

            // act
            session.Delete(entity);

            // assert
            Assert.True(entity.Deleted);
            Assert.True(entity2.Deleted);

            connectionMock.VerifyAll();
            entityServiceMock.VerifyAll();
        }

        [Fact]
        public void Save_Exception_When_Not_Saved_Entity_Is_Being_Deleted()
        {
            // arrange
            var connectionMock = TestHelper.SetupMock<IConnection>();
            var entity = new Artist {Deleted = true};
            var session = new Session(connectionMock.Object);

            // act
            Assert.Throws<InvalidOperationException>(() => session.Save(entity));

            // assert
            connectionMock.VerifyAll();
        }

        [Fact]
        public void Save_Insert_Entities_From_EntityService_Into_PersistenceContext()
        {
            // arrange
            var connectionMock = TestHelper.SetupMock<IConnection>();
            var session = new Session(connectionMock.Object);
            var entity = new Artist();

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.Save)).Returns(new List<Entity> {entity});

            // act
            session.Save(entity);

            // assert
            var persistenceContext = GetPersistenceContext(session);
            Assert.NotNull(persistenceContext.Lookup<Artist>(entity.Id));

            connectionMock.VerifyAll();
            entityServiceMock.VerifyAll();
        }

        [Fact]
        public void Get_Lookup_PersistenceContext()
        {
            // arrange
            var connectionMock = TestHelper.SetupMock<IConnection>();
            var session = new Session(connectionMock.Object);
            var entity = new Artist();
            var persistenceContext = GetPersistenceContext(session);
            
            persistenceContext.Insert(entity);

            // act
            var result = session.Get<Artist>(entity.Id);

            // assert
            Assert.Equal(entity, result);

            connectionMock.VerifyAll();
        }

        private PersistenceContext GetPersistenceContext(Session session)
        {
            return (PersistenceContext)session.GetType().GetField("_persistenceContext", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(session);
        }
    }
}

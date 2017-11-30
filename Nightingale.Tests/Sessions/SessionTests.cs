using System;
using System.Collections.Generic;
using System.Data;
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

        [Fact]
        public void BeginTransaction_Throw_Exception_When_Already_In_Transaction()
        {
            // arrange
            var transactionMock = TestHelper.SetupMock<IDbTransaction>();
            var connectionMock = TestHelper.SetupMock<IConnection>();
            connectionMock.Setup(s => s.BeginTransaction(IsolationLevel.Serializable)).Returns(transactionMock.Object);

            var session = new Session(connectionMock.Object);

            session.BeginTransaction();

            // act
            Assert.Throws<SessionException>(() => session.BeginTransaction());

            // assert
            connectionMock.VerifyAll();
            transactionMock.VerifyAll();
        }

        [Fact]
        public void BeginTransaction_Call_Session_Plugins_On_Commit()
        {
            // arrange
            var transactionMock = TestHelper.SetupMock<IDbTransaction>();
            transactionMock.Setup(s => s.Dispose());

            var connectionMock = TestHelper.SetupMock<IConnection>();
            connectionMock.Setup(s => s.BeginTransaction(IsolationLevel.Serializable)).Returns(transactionMock.Object);
            connectionMock.Setup(s => s.Commit());

            var session = new Session(connectionMock.Object);
            var sessionPluginMock = TestHelper.SetupMock<ISessionPlugin>();
            sessionPluginMock.Setup(s => s.Commit());

            session.SessionPlugins.Add(sessionPluginMock.Object);

            // act
            var transaction = session.BeginTransaction();
            transaction.Commit();

            // assert
            connectionMock.VerifyAll();
            transactionMock.VerifyAll();
            sessionPluginMock.VerifyAll();
        }

        [Fact]
        public void BeginTransaction_Commit_Allows_New_Transaction()
        {
            // arrange
            var transactionMock = TestHelper.SetupMock<IDbTransaction>();
            transactionMock.Setup(s => s.Dispose());

            var connectionMock = TestHelper.SetupMock<IConnection>();
            connectionMock.Setup(s => s.BeginTransaction(IsolationLevel.Serializable)).Returns(transactionMock.Object);
            connectionMock.Setup(s => s.Commit());

            var session = new Session(connectionMock.Object);

            // act
            var transaction = session.BeginTransaction();
            transaction.Commit();
            session.BeginTransaction();

            // assert
            connectionMock.VerifyAll();
            transactionMock.VerifyAll();
        }

        [Theory]
        [InlineData(IsolationLevel.Chaos)]
        [InlineData(IsolationLevel.ReadCommitted)]
        [InlineData(IsolationLevel.ReadUncommitted)]
        [InlineData(IsolationLevel.RepeatableRead)]
        [InlineData(IsolationLevel.Serializable)]
        [InlineData(IsolationLevel.Snapshot)]
        [InlineData(IsolationLevel.Unspecified)]
        public void BeginTransaction_Uses_Expected_IsolationLevel(IsolationLevel isolationLevel)
        {
            // arrange
            var transactionMock = TestHelper.SetupMock<IDbTransaction>();
            var connectionMock = TestHelper.SetupMock<IConnection>();
            connectionMock.Setup(s => s.BeginTransaction(isolationLevel)).Returns(transactionMock.Object);

            var session = new Session(connectionMock.Object);

            // act
            session.BeginTransaction(isolationLevel);

            // assert
            connectionMock.VerifyAll();
            transactionMock.VerifyAll();
        }

        [Fact]
        public void DiscardChanges_Work()
        {
            // arrange
            TestHelper.SetupEntityMetadataServices();

            var connectionMock = TestHelper.SetupMock<IConnection>();
            var session = new Session(connectionMock.Object);
            var entity = new Artist();
            var statisticValue = new ArtistStatisticValues();

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.Save)).Returns(new List<Entity> {entity});

            entity.Name = "Source";
            entity.StatisticValues.Add(statisticValue);
            entity.PropertyChangeTracker.Clear();

            session.Save(entity);

            entity.Name = "Test";
            entity.AnotherArtist = new Artist();
            entity.StatisticValues.Add(new ArtistStatisticValues());

            entity.PropertyChangeTracker.AddPropertyChangedItem<Artist>(x => x.Name, "Source", "Test");
            entity.PropertyChangeTracker.AddPropertyChangedItem<Artist>(x => x.AnotherArtist, null, entity.AnotherArtist);

            // act
            session.DiscardChanges();

            // assert
            Assert.Equal("Source", entity.Name);
            Assert.Null(entity.AnotherArtist);
            Assert.Single(entity.StatisticValues, statisticValue);

            connectionMock.VerifyAll();
        }

        private PersistenceContext GetPersistenceContext(Session session)
        {
            return (PersistenceContext)session.GetType().GetField("_persistenceContext", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(session);
        }
    }
}

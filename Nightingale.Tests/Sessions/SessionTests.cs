using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Moq;
using Nightingale.Entities;
using Nightingale.Sessions;
using Nightingale.Tests.DataSources;
using Xunit;
using Enumerable = System.Linq.Enumerable;

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
            TestHelper.SetupEntityMetadataServices();

            var connectionMock = TestHelper.SetupMock<IConnection>();
            var session = new Session(connectionMock.Object);
            var entity = new Artist();

            // act
            session.Delete(entity);

            // assert
            Assert.True(entity.Deleted);

            connectionMock.VerifyAll();
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
            TestHelper.SetupEntityMetadataServices();

            var connectionMock = TestHelper.SetupMock<IConnection>();
            var session = new Session(connectionMock.Object);
            var entity = new Artist();

            // act
            session.Save(entity);

            // assert
            var persistenceContext = GetPersistenceContext(session);
            Assert.NotNull(persistenceContext.Lookup<Artist>(entity.Id));

            connectionMock.VerifyAll();
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
        public void BeginTransaction_Creates_Nested_Transaction_When_Already_In_Transaction()
        {
            // arrange
            var transactionMock = TestHelper.SetupMock<IDbTransaction>();
            var connectionMock = TestHelper.SetupMock<IConnection>();
            connectionMock.Setup(s => s.BeginTransaction(IsolationLevel.Serializable)).Returns(transactionMock.Object);
            connectionMock.Setup(s => s.Save(It.IsAny<string>()));

            var session = new Session(connectionMock.Object);

            session.BeginTransaction();

            // act
            var result = session.BeginTransaction();

            // assert
            Assert.IsType<NestedTransaction>(result);

            connectionMock.VerifyAll();
            transactionMock.VerifyAll();
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

        [Fact]
        public void Save_Also_Attaches_The_Entity()
        {
            // arrange
            TestHelper.SetupEntityMetadataServices();

            var entity = new Artist();

            var connectionMock = TestHelper.SetupMock<IConnection>();
            var session = new Session(connectionMock.Object);

            // act
            session.Save(entity);

            // assert
            Assert.Equal(entity, (Artist)Enumerable.First(GetPersistenceContext(session)));

            connectionMock.VerifyAll();
        }

        [Fact]
        public void Save_Calls_SessionInterceptor()
        {
            // arrange
            TestHelper.SetupEntityMetadataServices();

            var entity = new Artist();

            var connectionMock = TestHelper.SetupMock<IConnection>();
            var session = new Session(connectionMock.Object);

            var interceptorMock = TestHelper.SetupMock<ISessionInterceptor>();
            interceptorMock.Setup(s => s.Save(entity)).Returns(true);

            session.Interceptors.Add(interceptorMock.Object);

            // act
            session.Save(entity);

            // assert
            connectionMock.VerifyAll();
            interceptorMock.VerifyAll();
        }

        [Fact]
        public void Delete_Calls_SessionInterceptor()
        {
            // arrange
            TestHelper.SetupEntityMetadataServices();

            var entity = new Artist();

            var connectionMock = TestHelper.SetupMock<IConnection>();
            var session = new Session(connectionMock.Object);

            var interceptorMock = TestHelper.SetupMock<ISessionInterceptor>();
            interceptorMock.Setup(s => s.Delete(entity)).Returns(true);

            session.Interceptors.Add(interceptorMock.Object);

            // act
            session.Delete(entity);

            // assert
            connectionMock.VerifyAll();
            interceptorMock.VerifyAll();
        }

        [Fact]
        public void SaveChanges_Validate_Entity()
        {
            // arrange
            TestHelper.SetupEntityMetadataServices();

            var entity = new Song();

            var connectionMock = TestHelper.SetupMock<IConnection>();
            var session = new Session(connectionMock.Object);
            session.Save(entity);

            // act
            Assert.Throws<SessionException>(() => session.SaveChanges());

            // assert
            connectionMock.VerifyAll();
        }

        [Fact]
        public void SaveChanges_Detect_Transient_Values()
        {
            // arrange
            TestHelper.SetupEntityMetadataServices();

            var entity = new Song {Title = "SongTitle"};
            entity.Artist = new Artist();

            var connectionMock = TestHelper.SetupMock<IConnection>();
            var session = new Session(connectionMock.Object);
            session.Save(entity);

            // act
            Assert.Throws<TransientEntityException>(() => session.SaveChanges());

            // assert
            connectionMock.VerifyAll();
        }

        [Fact]
        public void SaveChanges_Detect_Mandatory_Deleted_Field()
        {
            // arrange
            TestHelper.SetupEntityMetadataServices();

            var entity = new Book {Author = TestHelper.CreateEntityWithId<Author>(1)};
            TestHelper.MarkEntityDeleted(entity.Author);

            var connectionMock = TestHelper.SetupMock<IConnection>();
            var session = new Session(connectionMock.Object);
            session.Save(entity);

            // act
            Assert.Throws<InvalidOperationException>(() => session.SaveChanges());

            // assert
            connectionMock.VerifyAll();
        }

        [Fact]
        public void Clear_Discards_PersistenceContext()
        {
            // arrange
            var connectionMock = TestHelper.SetupMock<IConnection>();
            var session = new Session(connectionMock.Object);
            var persistenceContext = GetPersistenceContext(session);

            persistenceContext.Insert(new Artist());

            // act
            session.Clear();

            // assert
            Assert.Empty(persistenceContext);

            connectionMock.VerifyAll();
        }

        private PersistenceContext GetPersistenceContext(Session session)
        {
            return (PersistenceContext)session.GetType().GetField("_persistenceContext", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(session);
        }
    }
}

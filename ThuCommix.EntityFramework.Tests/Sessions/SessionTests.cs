using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Moq;
using NUnit.Framework;
using ThuCommix.EntityFramework.Entities;
using ThuCommix.EntityFramework.Metadata;
using ThuCommix.EntityFramework.Queries;
using ThuCommix.EntityFramework.Sessions;
using ThuCommix.EntityFramework.Tests.DataSources;

namespace ThuCommix.EntityFramework.Tests.Sessions
{
    [TestFixture]
    public class SessionTests
    {
        [SetUp]
        public void Setup()
        {
            DependencyResolver.Clear();
        }

        [Test]
        public void Session_Constructor_Opens_Connection()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupMock<IDataProvider>();
            dataProviderMock.Setup(s => s.Open());

            // act
            var session = new SessionProxy(dataProviderMock.Object);

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Session_Constructor_Verify_Default_Config()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();

            // act
            var session = new SessionProxy(dataProviderMock.Object);

            // assert
            Assert.That(session.DeletionMode, Is.EqualTo(DeletionMode.Soft));
            Assert.That(session.DebugMode, Is.True);
            Assert.That(session.FlushMode, Is.EqualTo(SessionFlushMode.Commit));

            dataProviderMock.VerifyAll();
        }

        [Test]
        public void BeginTransaction_Calls_DataProvider_BeginTransaction()
        {
            // arrange
            const IsolationLevel isolationlevel = IsolationLevel.Serializable;

            var dataProviderMock = TestHelper.SetupMock<IDataProvider>();
            var disposeableMock = TestHelper.SetupMock<IDisposable>();

            dataProviderMock.Setup(s => s.Open());
            dataProviderMock.Setup(s => s.BeginTransaction(isolationlevel)).Returns(disposeableMock.Object);

            var session = new SessionProxy(dataProviderMock.Object);

            // act
            var result = session.BeginTransaction(isolationlevel);

            // assert
            Assert.That(result, Is.TypeOf<TransactionProxy>());

            dataProviderMock.VerifyAll();
            disposeableMock.VerifyAll();
        }

        [Test]
        public void BeginTransaction_Dispose_Calls_Rollback()
        {
            // arrange
            const IsolationLevel isolationlevel = IsolationLevel.Serializable;

            var dataProviderMock = TestHelper.SetupMock<IDataProvider>();
            var disposeableMock = TestHelper.SetupMock<IDisposable>();

            disposeableMock.Setup(s => s.Dispose());

            dataProviderMock.Setup(s => s.Open());
            dataProviderMock.Setup(s => s.BeginTransaction(isolationlevel)).Returns(disposeableMock.Object);
            dataProviderMock.Setup(s => s.Rollback());

            var session = new SessionProxy(dataProviderMock.Object);

            // act
            using (session.BeginTransaction(isolationlevel))
            {
            }

            // assert
            dataProviderMock.VerifyAll();
            disposeableMock.VerifyAll();
        }

        [Test]
        public void BeginTransaction_Throws_Exception_When_Already_In_Transaction()
        {
            // arrange
            const IsolationLevel isolationlevel = IsolationLevel.Serializable;

            var dataProviderMock = TestHelper.SetupMock<IDataProvider>();
            var disposeableMock = TestHelper.SetupMock<IDisposable>();

            dataProviderMock.Setup(s => s.Open());
            dataProviderMock.Setup(s => s.BeginTransaction(isolationlevel)).Returns(disposeableMock.Object);

            var session = new SessionProxy(dataProviderMock.Object);

            // act
            session.BeginTransaction(isolationlevel);
            Assert.Throws<SessionException>(() => session.BeginTransaction(isolationlevel));

            // assert
            dataProviderMock.VerifyAll();
            disposeableMock.VerifyAll();
        }

        [Test]
        public void Rollback_Calls_DataProvider_Rollback()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupMock<IDataProvider>();
            var disposeableMock = TestHelper.SetupMock<IDisposable>();

            dataProviderMock.Setup(s => s.Rollback());
            dataProviderMock.Setup(s => s.Open());
            dataProviderMock.Setup(s => s.BeginTransaction(IsolationLevel.Serializable)).Returns(disposeableMock.Object);

            var session = new SessionProxy(dataProviderMock.Object);

            // act
            session.BeginTransaction(IsolationLevel.Serializable);
            session.Rollback();

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Rollback_Throws_Exception_When_Not_In_Transaction()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupMock<IDataProvider>();
            dataProviderMock.Setup(s => s.Open());

            var session = new SessionProxy(dataProviderMock.Object);

            // act
            Assert.Throws<SessionException>(() => session.Rollback());

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void RollbackTo_Throws_Exception_When_Not_In_Transaction()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupMock<IDataProvider>();
            dataProviderMock.Setup(s => s.Open());

            var session = new SessionProxy(dataProviderMock.Object);

            // act
            Assert.Throws<SessionException>(() => session.RollbackTo(string.Empty));

            // assert
            dataProviderMock.VerifyAll();
        }

        public void RollbackTo_Calls_DataProvider_RollbackTo()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupMock<IDataProvider>();
            var disposableMock = TestHelper.SetupMock<IDisposable>();

            dataProviderMock.Setup(s => s.Open());
            dataProviderMock.Setup(s => s.BeginTransaction(IsolationLevel.Serializable)).Returns(disposableMock.Object);
            dataProviderMock.Setup(s => s.RollbackTo(string.Empty));

            var session = new SessionProxy(dataProviderMock.Object);

            // act
            session.BeginTransaction(IsolationLevel.Serializable);
            session.RollbackTo(string.Empty);

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Release_Throws_Exception_When_Not_In_Transaction()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupMock<IDataProvider>();
            dataProviderMock.Setup(s => s.Open());

            var session = new SessionProxy(dataProviderMock.Object);

            // act
            Assert.Throws<SessionException>(() => session.Release(string.Empty));

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Release_Calls_DataProvider_Release()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupMock<IDataProvider>();
            var disposableMock = TestHelper.SetupMock<IDisposable>();

            dataProviderMock.Setup(s => s.Open());
            dataProviderMock.Setup(s => s.BeginTransaction(IsolationLevel.Serializable)).Returns(disposableMock.Object);
            dataProviderMock.Setup(s => s.Release(string.Empty));

            var session = new SessionProxy(dataProviderMock.Object);

            // act
            session.BeginTransaction(IsolationLevel.Serializable);
            session.Release(string.Empty);

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Commit_Throws_Exception_When_Not_In_Transaction()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupMock<IDataProvider>();
            dataProviderMock.Setup(s => s.Open());

            var session = new SessionProxy(dataProviderMock.Object);

            // act
            Assert.Throws<SessionException>(() => session.Commit());

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Commit_Calls_DataProvider_Commit()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupMock<IDataProvider>();
            var disposableMock = TestHelper.SetupMock<IDisposable>();

            dataProviderMock.Setup(s => s.Open());
            dataProviderMock.Setup(s => s.BeginTransaction(IsolationLevel.Serializable)).Returns(disposableMock.Object);
            dataProviderMock.Setup(s => s.Commit());

            var session = new SessionProxy(dataProviderMock.Object);

            // act
            session.BeginTransaction(IsolationLevel.Serializable);
            session.Commit();

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Save_Throws_Exception_When_Not_In_Transaction()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupMock<IDataProvider>();
            dataProviderMock.Setup(s => s.Open());

            var session = new SessionProxy(dataProviderMock.Object);

            // act
            Assert.Throws<SessionException>(() => session.Save(string.Empty));

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Save_Calls_DataProvider_Save()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupMock<IDataProvider>();
            var disposableMock = TestHelper.SetupMock<IDisposable>();

            dataProviderMock.Setup(s => s.Open());
            dataProviderMock.Setup(s => s.BeginTransaction(IsolationLevel.Serializable)).Returns(disposableMock.Object);
            dataProviderMock.Setup(s => s.Save(string.Empty));

            var session = new SessionProxy(dataProviderMock.Object);

            // act
            session.BeginTransaction(IsolationLevel.Serializable);
            session.Save(string.Empty);

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Evict_Set_Flag_On_Entity_And_Remove_From_Flushlist()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);
            var entityServiceMock = TestHelper.SetupMock<IEntityService>();

            var entity = new Artist();

            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.Save)).Returns(new List<Entity> { entity });

            session.SaveOrUpdate(entity);
            var flushListCountBeforeEvict = session.CallGetFlushList().Count;

            // act
            session.Evict(entity);

            // assert
            Assert.That(flushListCountBeforeEvict, Is.EqualTo(1));
            Assert.That(TestHelper.CheckEvicted(entity), Is.True);
            Assert.That(session.CallGetFlushList(), Is.Empty);

            dataProviderMock.VerifyAll();
        }

        [Test]
        public void SaveOrUpdate_Prevent_Saving_Of_Deleted_Entity()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var artist = new Artist();
            TestHelper.MarkEntityDeleted(artist);

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(artist, Cascade.Save)).Returns(new List<Entity> { artist });

            // act
            Assert.Throws<SessionException>(() => session.SaveOrUpdate(artist));

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void SaveOrUpdate_Resolved_Child_Entities()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            entity.AnotherArtist = new Artist();

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.Save)).Returns(new List<Entity> { entity, entity.AnotherArtist });

            // act
            session.SaveOrUpdate(entity);

            // assert
            Assert.That(session.CallGetFlushList().Count, Is.EqualTo(2));
            Assert.That(session.CallGetFlushList()[0], Is.EqualTo(entity));
            Assert.That(session.CallGetFlushList()[1], Is.EqualTo(entity.AnotherArtist));

            dataProviderMock.VerifyAll();
        }

        [Test]
        public void ExecuteQuery_Throws_When_Query_Is_Null()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            // act
            Assert.Throws<ArgumentNullException>(() => session.ExecuteQuery(null));

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void ExecuteQuery_Creates_Entity_Result_List()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();

            var canRead = true;

            var query = Query.CreateQuery<Artist>();

            dataReaderMock.Setup(s => s.Read()).Returns(() => { if (canRead) { canRead = false; return true; } return false; });
            dataProviderMock.Setup(s => s.ExecuteReader(query)).Returns(dataReaderMock.Object);
            dataReaderMock.Setup(s => s.Close());
            dataReaderMock.Setup(s => s.Dispose());

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.CreateEntity(dataReaderMock.Object, query.EntityType)).Returns(new Artist());

            // act
            var result = session.ExecuteQuery(query);

            // assert
            Assert.That(result.Count, Is.EqualTo(1));

            dataProviderMock.VerifyAll();
            dataReaderMock.VerifyAll();
        }

        [Test]
        public void Load_Creates_Expected_Query_And_Calls_ExecuteQuery()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();
            var session = new SessionProxy(dataProviderMock.Object);

            TestHelper.SetupEntityMetadataServices();

            const int Id = 1;

            Query query = null;

            dataProviderMock.Setup(s => s.ExecuteReader(It.IsAny<IQuery>())).Returns(dataReaderMock.Object).Callback<IQuery>((q) => query = q as Query);
            dataReaderMock.Setup(s => s.Read()).Returns(false);
            dataReaderMock.Setup(s => s.Close());
            dataReaderMock.Setup(s => s.Dispose());

            // act
            var result = session.Load(Id, typeof(Artist));

            // assert
            Assert.That(result, Is.Null);
            Assert.That(query, Is.Not.Null);
            Assert.That(query.ConditionGroups.First().Conditions.Any(x => x.PropertyPath == "Id" && (int)x.EquationValue == Id), Is.True);
            Assert.That(query.ConditionGroups.First().Conditions.Any(x => x.PropertyPath == "Deleted" && (bool)x.EquationValue == false), Is.True);
            Assert.That(query.EntityType, Is.EqualTo(typeof(Artist)));

            dataReaderMock.VerifyAll();
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Flush_Throws_Exception_When_Entity_Is_Not_Valid()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var entity = new Artist();

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.Save)).Returns(new List<Entity> { entity });

            TestHelper.SetupEntityMetadataServices();

            // act
            session.SaveOrUpdate(entity);
            Assert.Throws<SessionException>(() => session.Flush());

            // assert
            entityServiceMock.VerifyAll();
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Flush_Insert_Entity_If_Not_Saved()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var entity = new Artist { Name = "Test" };

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.Save)).Returns(new List<Entity> { entity });

            TestHelper.SetupEntityMetadataServices();

            // act
            session.SaveOrUpdate(entity);
            session.Flush();

            // assert
            Assert.That(session.PerformInsertCalled, Is.True);
            Assert.That(entity.Id, Is.EqualTo(1));

            entityServiceMock.VerifyAll();
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Flush_Throws_Exception_When_Entity_Is_Not_Saved_After_Insert()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var entity = new Artist { Name = "Test" };

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.Save)).Returns(new List<Entity> { entity });

            TestHelper.SetupEntityMetadataServices();

            session.EntityNotSavedAfterInsert = true;

            // act
            session.SaveOrUpdate(entity);
            Assert.Throws<SessionInsertException>(() => session.Flush());

            // assert
            Assert.That(session.PerformInsertCalled, Is.True);
            Assert.That(entity.Id, Is.EqualTo(0));

            entityServiceMock.VerifyAll();
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Flush_Throws_Exception_When_Insert_Fails()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var entity = new Artist { Name = "Test" };

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.Save)).Returns(new List<Entity> { entity });

            TestHelper.SetupEntityMetadataServices();

            session.ThrowExceptionOnInsert = true;

            // act
            session.SaveOrUpdate(entity);
            Assert.Throws<SessionInsertException>(() => session.Flush());

            // assert
            Assert.That(session.PerformInsertCalled, Is.True);
            Assert.That(entity.Id, Is.EqualTo(0));

            entityServiceMock.VerifyAll();
            dataProviderMock.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Flush_Update_Only_When_Entity_Has_Changes(bool hasChanges)
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            entity.Name = string.Empty;

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.Save)).Returns(new List<Entity> { entity });

            TestHelper.SetupEntityMetadataServices();

            if(hasChanges)
            {
                entity.PropertyChangeTracker.AddPropertyChangedItem("Name", string.Empty, "Test");
                entityServiceMock.Setup(s => s.UpdateForeignFields(entity));
            }

            // act
            session.SaveOrUpdate(entity);
            session.Flush();

            // assert
            Assert.That(session.PerformUpdateCalled, Is.EqualTo(hasChanges));

            entityServiceMock.VerifyAll();
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Flush_Update_Increases_Version()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            entity.Name = string.Empty;

            var oldVersion = entity.Version;

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.Save)).Returns(new List<Entity> { entity });

            TestHelper.SetupEntityMetadataServices();

            entity.PropertyChangeTracker.AddPropertyChangedItem("Name", string.Empty, "Test");
            entityServiceMock.Setup(s => s.UpdateForeignFields(entity));

            // act
            session.SaveOrUpdate(entity);
            session.Flush();

            // assert
            Assert.That(session.PerformUpdateCalled, Is.True);
            Assert.That(oldVersion + 1, Is.EqualTo(entity.Version));

            entityServiceMock.VerifyAll();
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Flush_Update_On_Error_Restore_Version()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            entity.Name = string.Empty;

            var oldVersion = entity.Version;

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.Save)).Returns(new List<Entity> { entity });

            TestHelper.SetupEntityMetadataServices();

            entity.PropertyChangeTracker.AddPropertyChangedItem("Name", string.Empty, "Test");
            entityServiceMock.Setup(s => s.UpdateForeignFields(entity));

            session.ThrowExceptionOnUpdate = true;

            // act
            session.SaveOrUpdate(entity);
            Assert.Throws<SessionUpdateException>(() => session.Flush());

            // assert
            Assert.That(session.PerformUpdateCalled, Is.True);
            Assert.That(oldVersion, Is.EqualTo(entity.Version));

            entityServiceMock.VerifyAll();
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Flush_Update_Clear_PropertyChangeTracker()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            entity.Name = string.Empty;

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.Save)).Returns(new List<Entity> { entity });

            TestHelper.SetupEntityMetadataServices();

            entity.PropertyChangeTracker.AddPropertyChangedItem("Name", string.Empty, "Test");
            entityServiceMock.Setup(s => s.UpdateForeignFields(entity));

            // act
            session.SaveOrUpdate(entity);
            session.Flush();

            // assert
            Assert.That(entity.PropertyChangeTracker.HasChanges, Is.False);

            entityServiceMock.VerifyAll();
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Delete_Does_Nothing_On_DeletionMode_None()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var entity = TestHelper.CreateEntityWithId<Artist>(1);

            session.DeletionMode = DeletionMode.None;

            // act
            session.Delete(entity);

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Delete_Set_Entity_Deleted_And_Call_SaveOrUpdate_In_SoftMode()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();

            var entity = TestHelper.CreateEntityWithId<Artist>(1);

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.SaveDelete)).Returns(new List<Entity> { entity });
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.Save)).Returns(new List<Entity> { entity });

            dataProviderMock.Setup(s => s.ExecuteReader(It.IsAny<IQuery>())).Returns(dataReaderMock.Object);
            dataReaderMock.Setup(s => s.Read()).Returns(false);
            dataReaderMock.Setup(s => s.Close());
            dataReaderMock.Setup(s => s.Dispose());

            TestHelper.SetupEntityMetadataServices();

            session.DeletionMode = DeletionMode.Soft;

            // act
            session.Delete(entity);

            // assert
            Assert.That(entity.Deleted, Is.True);
            Assert.That(session.CallGetFlushList()[0], Is.EqualTo(entity));

            dataReaderMock.VerifyAll();
            entityServiceMock.VerifyAll();
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Delete_Set_Entity_Deleted_And_Call_PerformDelete_In_HardMode()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();

            var entity = TestHelper.CreateEntityWithId<Artist>(1);

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.SaveDelete)).Returns(new List<Entity> { entity });

            dataProviderMock.Setup(s => s.ExecuteReader(It.IsAny<IQuery>())).Returns(dataReaderMock.Object);
            dataReaderMock.Setup(s => s.Read()).Returns(false);
            dataReaderMock.Setup(s => s.Close());
            dataReaderMock.Setup(s => s.Dispose());

            TestHelper.SetupEntityMetadataServices();

            session.DeletionMode = DeletionMode.Hard;

            // act
            session.Delete(entity);

            // assert
            Assert.That(entity.Deleted, Is.True);
            Assert.That(session.PerformDeleteCalled, Is.True);

            dataReaderMock.VerifyAll();
            entityServiceMock.VerifyAll();
            dataProviderMock.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Delete_Set_Hard_Delete_Is_Only_Called_For_Saved_Entities(bool saved)
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();

            var entity = TestHelper.CreateEntityWithId<Artist>(saved ? 1 : 0);

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.SaveDelete)).Returns(new List<Entity> { entity });

            dataProviderMock.Setup(s => s.ExecuteReader(It.IsAny<IQuery>())).Returns(dataReaderMock.Object);
            dataReaderMock.Setup(s => s.Read()).Returns(false);
            dataReaderMock.Setup(s => s.Close());
            dataReaderMock.Setup(s => s.Dispose());

            TestHelper.SetupEntityMetadataServices();

            session.DeletionMode = DeletionMode.Hard;

            // act
            session.Delete(entity);

            // assert
            Assert.That(entity.Deleted, Is.True);
            Assert.That(session.PerformDeleteCalled, Is.EqualTo(saved));

            dataReaderMock.VerifyAll();
            entityServiceMock.VerifyAll();
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void Delete_Throws_If_Entity_Is_Referenced_Somewhere()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();

            var entity = TestHelper.CreateEntityWithId<Artist>(1);

            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.GetChildEntities(entity, Cascade.SaveDelete)).Returns(new List<Entity> { entity });

            var canRead = true;

            dataProviderMock.Setup(s => s.ExecuteReader(It.IsAny<IQuery>())).Returns(dataReaderMock.Object);
            dataReaderMock.Setup(s => s.Read()).Returns(() => { if (canRead) { canRead = false; return true; } return false; });
            dataReaderMock.Setup(s => s.Close());
            dataReaderMock.Setup(s => s.Dispose());

            entityServiceMock.Setup(s => s.CreateEntity(dataReaderMock.Object, typeof(ArtistStatisticValues))).Returns(new ArtistStatisticValues());

            TestHelper.SetupEntityMetadataServices();

            session.DeletionMode = DeletionMode.Hard;

            // act
            var result = Assert.Throws<SessionDeleteException>(() => session.Delete(entity));
            Assert.That(entity.Deleted, Is.False);
            Assert.That(result.ConstraintViolations.Count, Is.EqualTo(1));
            Assert.That(result.ConstraintViolations[0], Is.EqualTo("Entity 'ArtistStatisticValues' with Id = '0' references 'Artist' in 'FK_AnotherArtist_ID'."));

            // assert
            dataReaderMock.VerifyAll();
            entityServiceMock.VerifyAll();
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void PerformDelete_Throws_Exception_If_Entity_Could_Not_Be_Deleted()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var entity = TestHelper.CreateEntityWithId<Artist>(1);

            dataProviderMock.Setup(s => s.ExecuteNonQuery(It.IsAny<IQuery>())).Returns(0);

            TestHelper.SetupEntityMetadataServices();

            // act
            Assert.Throws<SessionDeleteException>(() => session.CallPerformDelete(entity));

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void PerformDelete_Expected_Query_Command()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var entity = TestHelper.CreateEntityWithId<Artist>(1);

            IQuery query = null;

            dataProviderMock.Setup(s => s.ExecuteNonQuery(It.IsAny<IQuery>())).Returns(1).Callback<IQuery>((q) => query = q);

            TestHelper.SetupEntityMetadataServices();

            // act
            session.CallPerformDelete(entity);

            // assert
            Assert.That(query, Is.Not.Null);
            Assert.That(query.Command, Is.EqualTo("DELETE Artist WHERE Id = 1 AND Version = 1"));
            Assert.That(query.EntityType, Is.EqualTo(typeof(Artist)));

            dataProviderMock.VerifyAll();
        }

        [Test]
        public void PerformUpdate_Throws_Exception_When_No_Rows_Changed()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var entity = TestHelper.CreateEntityWithId<Artist>(1);

            dataProviderMock.Setup(s => s.ExecuteNonQuery(It.IsAny<IQuery>())).Returns(0);

            entity.PropertyChangeTracker.AddPropertyChangedItem("Name", null, string.Empty);

            TestHelper.SetupEntityMetadataServices();

            // act
            Assert.Throws<SessionUpdateException>(() => session.CallPerformUpdate(entity));

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void PerformUpdate_Returns_When_Entity_Has_No_Changes()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var entity = TestHelper.CreateEntityWithId<Artist>(1);

            TestHelper.SetupEntityMetadataServices();

            // act
            session.CallPerformUpdate(entity);

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void PerformUpdate_Expected_Query_Command()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var entity = TestHelper.CreateEntityWithId<Artist>(1);

            IQuery query = null;

            dataProviderMock.Setup(s => s.ExecuteNonQuery(It.IsAny<IQuery>())).Returns(1).Callback<IQuery>((q) => query = q);

            entity.PropertyChangeTracker.AddPropertyChangedItem("Name", null, "Name");
            entity.PropertyChangeTracker.AddPropertyChangedItem("Alias", null, "Alias");

            entity.Name = "Name";
            entity.Alias = "Alias";

            TestHelper.SetupEntityMetadataServices();

            // act
            session.CallPerformUpdate(entity);

            // assert
            Assert.That(query, Is.Not.Null);
            Assert.That(query.Command, Is.EqualTo("UPDATE Artist SET Name = @Name,Alias = @Alias WHERE Id = 1 AND Version = 0"));
            Assert.That(query.EntityType, Is.EqualTo(typeof(Artist)));
            Assert.That(query.Parameters.First().Value, Is.EqualTo("Name"));
            Assert.That(query.Parameters.Last().Value, Is.EqualTo("Alias"));

            dataProviderMock.VerifyAll();
        }

        [Test]
        public void PerformInsert_Expected_Query_Command()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var entity = new Artist();
            entity.Name = "TestName";
            entity.Alias = "TestAlias";

            IQuery query = null;

            dataProviderMock.Setup(s => s.ExecuteInsert(It.IsAny<IQuery>())).Returns(1).Callback<IQuery>((q) => query = q);

            TestHelper.SetupEntityMetadataServices();

            // act
            var result = session.CallPerformInsert(entity);

            // assert
            Assert.That(result, Is.EqualTo(1));
            Assert.That(query, Is.Not.Null);
            Assert.That(query.Command, Is.EqualTo("INSERT INTO Artist (Name,Alias,BirthDate,DeathDate,WebLink,Biography,Note,Label,FK_AnotherArtist_ID,Deleted,Version) VALUES (@Name,@Alias,@BirthDate,@DeathDate,@WebLink,@Biography,@Note,@Label,@FK_AnotherArtist_ID,@Deleted,@Version);"));
            Assert.That(query.EntityType, Is.EqualTo(typeof(Artist)));
            Assert.That(query.Parameters.Any(x => x.Name == "@Name" && (string)x.Value == entity.Name), Is.True);
            Assert.That(query.Parameters.Any(x => x.Name == "@Alias" && (string)x.Value == entity.Alias), Is.True);

            dataProviderMock.VerifyAll();
        }

        private class SessionProxy : Session
        {
            public bool PerformInsertCalled { get; private set; }

            public bool EntityNotSavedAfterInsert { get; set; }

            public bool ThrowExceptionOnInsert { get; set; }

            public bool PerformUpdateCalled { get; private set; }

            public bool ThrowExceptionOnUpdate { get; set; }

            public bool PerformDeleteCalled { get; set; }

            public SessionProxy(IDataProvider dataProvider) : base(dataProvider)
            {
            }

            public List<Entity> CallGetFlushList()
            {
                return GetFlushList();
            }

            protected override int PerformInsert(Entity entity)
            {
                PerformInsertCalled = true;

                if (ThrowExceptionOnInsert)
                    throw new Exception();

                return EntityNotSavedAfterInsert ? 0 : 1;
            }

            protected override void PerformUpdate(Entity entity)
            {
                PerformUpdateCalled = true;

                if (ThrowExceptionOnUpdate)
                    throw new Exception();
            }

            protected override void PerformDelete(Entity entity)
            {
                PerformDeleteCalled = true;
            }

            public void CallPerformDelete(Entity entity)
            {
                base.PerformDelete(entity);
            }

            public void CallPerformUpdate(Entity entity)
            {
                base.PerformUpdate(entity);
            }

            public int CallPerformInsert(Entity entity)
            {
                return base.PerformInsert(entity);
            }
        }
    }
}

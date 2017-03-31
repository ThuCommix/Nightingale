using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using ThuCommix.EntityFramework.Entities;
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

            var entity = new Artist();

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

            // act
            Assert.Throws<SessionException>(() => session.SaveOrUpdate(artist));

            // assert
            dataProviderMock.VerifyAll();
        }

        [Test]
        public void SaveOrUpdate_Throws_Exception_When_Entity_Is_Evicted()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);

            var entity = TestHelper.CreateEntityWithId<Artist>(1);

            // act
            session.Evict(entity);
            Assert.Throws<SessionException>(() => session.SaveOrUpdate(entity));

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

        [Test, Ignore]
        public void ExecuteQuery_Creates_Entity_Result_List()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();
            var session = new SessionProxy(dataProviderMock.Object);
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();

            var canRead = true;

            var query = Query.CreateQuery<Artist>();

            dataReaderMock.Setup(s => s.Read()).Returns(canRead).Callback(() => canRead = false);
            TestHelper.SetupDataReaderEntityBaseProperties(dataReaderMock);
            dataProviderMock.Setup(s => s.ExecuteReader(query)).Returns(dataReaderMock.Object);

            // act
            var result = session.ExecuteQuery(query);

            // assert
            Assert.That(result.Count, Is.EqualTo(1));

            dataProviderMock.VerifyAll();
            dataReaderMock.VerifyAll();
        }

        private class SessionProxy : Session
        {
            public SessionProxy(IDataProvider dataProvider) : base(dataProvider)
            {
            }

            public List<Entity> CallGetFlushList()
            {
                return GetFlushList();
            }
        }
    }
}

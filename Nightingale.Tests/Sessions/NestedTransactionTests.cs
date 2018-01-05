using System;
using Moq;
using Nightingale.Sessions;
using Xunit;

namespace Nightingale.Tests.Sessions
{
    public class NestedTransactionTests
    {
        public NestedTransactionTests()
        {
            DependencyResolver.Clear();
        }

        [Fact]
        public void NestedTransaction_Creates_SavePoint()
        {
            // arrange
            var parentTransactionMock = TestHelper.SetupMock<ITransaction>();
            parentTransactionMock.Setup(s => s.Save(It.IsAny<string>()));

            // act
            new NestedTransaction(parentTransactionMock.Object);

            // assert
            parentTransactionMock.VerifyAll();
        }

        [Fact]
        public void Commit_Call_Release_On_Parent_Transaction()
        {
            // arrange
            var savePoint = string.Empty;
            var parentTransactionMock = TestHelper.SetupMock<ITransaction>();
            parentTransactionMock.Setup(s => s.Save(It.IsAny<string>())).Callback<string>(s => savePoint = s);

            var nestedTransaction = new NestedTransaction(parentTransactionMock.Object);

            parentTransactionMock.Setup(s => s.Release(savePoint));

            // act
            nestedTransaction.Commit();

            // assert
            parentTransactionMock.VerifyAll();
        }

        [Fact]
        public void Commit_Call_Events()
        {
            // arrange
            var comitting = false;
            var committed = false;
            var finished = false;

            var parentTransactionMock = TestHelper.SetupMock<ITransaction>();
            parentTransactionMock.Setup(s => s.Save(It.IsAny<string>()));
            parentTransactionMock.Setup(s => s.Release(It.IsAny<string>()));

            var nestedTransaction = new NestedTransaction(parentTransactionMock.Object);
            nestedTransaction.Committing += (s, e) => comitting = true;
            nestedTransaction.Committed += (s, e) => committed = true;
            nestedTransaction.Finished += (s, e) => finished = true;

            // act
            nestedTransaction.Commit();

            // assert
            Assert.True(comitting);
            Assert.True(committed);
            Assert.True(finished);

            parentTransactionMock.VerifyAll();
        }

        [Fact]
        public void Commit_Throws_When_Not_In_Transaction()
        {
            // arrange
            var parentTransactionMock = TestHelper.SetupMock<ITransaction>();
            parentTransactionMock.Setup(s => s.Save(It.IsAny<string>()));
            parentTransactionMock.Setup(s => s.RollbackTo(It.IsAny<string>()));

            var nestedTransaction = new NestedTransaction(parentTransactionMock.Object);
            nestedTransaction.Rollback();

            // act
            Assert.Throws<InvalidOperationException>(() => nestedTransaction.Commit());

            // assert
            parentTransactionMock.VerifyAll();
        }

        [Fact]
        public void Dispose_Rollback_Prevented_When_Commit_Was_Called()
        {
            // arrange
            var parentTransactionMock = TestHelper.SetupMock<ITransaction>();
            parentTransactionMock.Setup(s => s.Save(It.IsAny<string>()));
            parentTransactionMock.Setup(s => s.Release(It.IsAny<string>()));

            var nestedTransaction = new NestedTransaction(parentTransactionMock.Object);
            nestedTransaction.Commit();

            // act
            nestedTransaction.Dispose();

            // assert
            parentTransactionMock.VerifyAll();
        }

        [Fact]
        public void Rollback_Throws_When_Not_In_Transaction()
        {
            // arrange
            var parentTransactionMock = TestHelper.SetupMock<ITransaction>();
            parentTransactionMock.Setup(s => s.Save(It.IsAny<string>()));
            parentTransactionMock.Setup(s => s.Release(It.IsAny<string>()));

            var nestedTransaction = new NestedTransaction(parentTransactionMock.Object);
            nestedTransaction.Commit();

            // act
            Assert.Throws<InvalidOperationException>(() => nestedTransaction.Rollback());

            // assert
            parentTransactionMock.VerifyAll();
        }

        [Fact]
        public void Rollback_Calls_RollbackTo_On_Parent_Transaction()
        {
            // arrange
            var parentTransactionMock = TestHelper.SetupMock<ITransaction>();
            parentTransactionMock.Setup(s => s.Save(It.IsAny<string>()));
            parentTransactionMock.Setup(s => s.RollbackTo(It.IsAny<string>()));

            var nestedTransaction = new NestedTransaction(parentTransactionMock.Object);

            // act
            nestedTransaction.Rollback();

            // assert
            parentTransactionMock.VerifyAll();
        }

        [Fact]
        public void Rollback_Calls_Event()
        {
            // arrange
            var finished = false;

            var parentTransactionMock = TestHelper.SetupMock<ITransaction>();
            parentTransactionMock.Setup(s => s.Save(It.IsAny<string>()));
            parentTransactionMock.Setup(s => s.RollbackTo(It.IsAny<string>()));

            var nestedTransaction = new NestedTransaction(parentTransactionMock.Object);
            nestedTransaction.Finished += (s, e) => finished = true;

            // act
            nestedTransaction.Rollback();

            // assert
            Assert.True(finished);

            parentTransactionMock.VerifyAll();
        }

        [Fact]
        public void Save_Throws_NotSupportedException()
        {
            // arrange
            var parentTransactionMock = TestHelper.SetupMock<ITransaction>();
            parentTransactionMock.Setup(s => s.Save(It.IsAny<string>()));

            var nestedTransaction = new NestedTransaction(parentTransactionMock.Object);

            // act
            Assert.Throws<NotSupportedException>(() => nestedTransaction.Save(string.Empty));

            // assert
            parentTransactionMock.VerifyAll();
        }

        [Fact]
        public void RollbackTo_Throws_NotSupportedException()
        {
            // arrange
            var parentTransactionMock = TestHelper.SetupMock<ITransaction>();
            parentTransactionMock.Setup(s => s.Save(It.IsAny<string>()));

            var nestedTransaction = new NestedTransaction(parentTransactionMock.Object);

            // act
            Assert.Throws<NotSupportedException>(() => nestedTransaction.RollbackTo(string.Empty));

            // assert
            parentTransactionMock.VerifyAll();
        }

        [Fact]
        public void Release_Throws_NotSupportedException()
        {
            // arrange
            var parentTransactionMock = TestHelper.SetupMock<ITransaction>();
            parentTransactionMock.Setup(s => s.Save(It.IsAny<string>()));

            var nestedTransaction = new NestedTransaction(parentTransactionMock.Object);

            // act
            Assert.Throws<NotSupportedException>(() => nestedTransaction.Release(string.Empty));

            // assert
            parentTransactionMock.VerifyAll();
        }
    }
}

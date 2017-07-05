using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Concordia.Framework.Sessions;

namespace Concordia.Framework.Tests.Sessions
{
    [TestFixture]
    public class SessionFactoryTests
    {
        [SetUp]
        public void Setup()
        {
            DependencyResolver.Clear();
            ((List<Session>)SessionFactory.Sessions).Clear();
        }

        [Test]
        public void OpenSession_Adds_Session()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();

            // act
            var result = SessionFactory.OpenSession(dataProviderMock.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(SessionFactory.Sessions.Count(), Is.EqualTo(1));

            dataProviderMock.VerifyAll();
        }

        [Test]
        public void OpenSessionT_Adds_Session()
        {
            // arrange
            var dataProviderMock = TestHelper.SetupDataProvider();

            // act
            var result = SessionFactory.OpenSession<StatelessSession>(dataProviderMock.Object);

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(SessionFactory.Sessions.Count(), Is.EqualTo(1));

            dataProviderMock.VerifyAll();
        }
    }
}

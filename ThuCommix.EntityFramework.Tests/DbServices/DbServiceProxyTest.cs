using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ThuCommix.EntityFramework.DbServices;
using ThuCommix.EntityFramework.Queries;

namespace ThuCommix.EntityFramework.Tests.DbServices
{
    [TestFixture]
    public class DbServiceProxyTest
    {
        [SetUp]
        public void Setup()
        {
            DependencyResolver.Clear();
        }

        [Test]
        public void DbServiceProxy_Calls_ExecuteFunc_Without_Parameters()
        {
            // arrange
            var sessionMock = TestHelper.CreateSessionMock();
            var service = DbServiceFactory.CreateService<ICustomDbService>(sessionMock.Object);

            sessionMock.Setup(s => s.ExecuteFunc<int>("dbo.getAdminId")).Returns(1);

            // act
            var result = service.GetAdminId();

            // assert
            Assert.That(result, Is.EqualTo(1));
            sessionMock.VerifyAll();
        }

        [Test]
        public void DbServiceProxy_Calls_ExecuteFunc_With_Correct_Parameters()
        {
            // arrange
            var sessionMock = TestHelper.CreateSessionMock();
            var service = DbServiceFactory.CreateService<ICustomDbService>(sessionMock.Object);
            QueryParameter[] parameters = null;

            sessionMock.Setup(s => s.ExecuteFunc<string>("dbo.getSecret", It.IsAny<QueryParameter[]>()))
                .Returns(string.Empty)
                .Callback<string, QueryParameter[]>((name, p) => parameters = p);

            // act
            var result = service.GetSecret("root", "root");

            // assert
            Assert.That(result, Is.EqualTo(string.Empty));
            Assert.That(parameters[0].Value, Is.EqualTo("root"));
            Assert.That(parameters[1].Value, Is.EqualTo("root"));
            sessionMock.VerifyAll();
        }

        private interface ICustomDbService : IDbService
        {
            [Procedure("dbo.getAdminId")]
            int GetAdminId();

            [Procedure("dbo.getSecret")]
            [Parameter("@User")]
            [Parameter("@Password")]
            string GetSecret(string username, string password);
        }
    }
}

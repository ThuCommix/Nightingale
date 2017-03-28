using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Moq;
using ThuCommix.EntityFramework.Entities;
using ThuCommix.EntityFramework.Sessions;

namespace ThuCommix.EntityFramework.Tests
{
    public static class TestHelper
    {
        public static Stream GetResourceStream(string name)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("ThuCommix.EntityFramework.Tests." + name);
        }

        public static Mock<T> SetupMock<T>() where T : class
        {
            return new Mock<T>(MockBehavior.Strict);
        }

        public static Mock<T> SetupMock<T>(params object[] parameters) where T : class
        {
            return new Mock<T>(MockBehavior.Strict, parameters);
        }

        public static T CreateEntityWithId<T>(int id) where T : Entity
        {
            var entity = (T)Activator.CreateInstance(typeof(T));
            entity.GetType().GetProperty("Id").SetValue(entity, id);

            return entity;
        }

        public static Mock<Session> CreateSessionMock()
        {
            var dataProviderMock = SetupMock<IDataProvider>();
            dataProviderMock.Setup(s => s.Open());

            var session = SetupMock<Session>(dataProviderMock.Object);

            return session;
        }
    }
}

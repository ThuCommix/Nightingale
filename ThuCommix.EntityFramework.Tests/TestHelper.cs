using System;
using System.Collections.Generic;
using System.Data;
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
            var mock = new Mock<T>(MockBehavior.Strict);
            RegisterMockInDependencyResolver(mock);

            return mock;
        }

        public static Mock<T> SetupMock<T>(params object[] parameters) where T : class
        {
            var mock = new Mock<T>(MockBehavior.Strict, parameters);
            RegisterMockInDependencyResolver(mock);

            return mock;
        }

        private static void RegisterMockInDependencyResolver<T>(Mock<T> mock) where T : class
        {
            DependencyResolver.Register(mock.Object);
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

        public static Mock<IDataProvider> SetupDataProvider()
        {
            var dataProviderMock = SetupMock<IDataProvider>();
            dataProviderMock.Setup(s => s.Open());

            return dataProviderMock;
        }

        public static bool CheckEvicted(Entity entity)
        {
            return (bool)entity.GetType().GetProperty("Evicted", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(entity);
        }

        public static void MarkEntityDeleted(Entity entity)
        {
            entity.GetType().GetProperty("Deleted").SetValue(entity, true);
        }

        public static void SetupDataReaderEntityBaseProperties(Mock<IDataReader> dataReader)
        {
            /*dataReader.Setup(s => s["Id"]).Returns(1);
            dataReader.Setup(s => s["Version"]).Returns(1);
            dataReader.Setup(s => s["Deleted"]).Returns(false);*/
        }
    }
}

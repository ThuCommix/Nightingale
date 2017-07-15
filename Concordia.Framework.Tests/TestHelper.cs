using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Concordia.Framework.Entities;
using Concordia.Framework.Metadata;
using Concordia.Framework.Queries;
using Concordia.Framework.Queries.Tokens;
using Concordia.Framework.Sessions;
using Concordia.Framework.Tests.DataSources;

namespace Concordia.Framework.Tests
{
    public static class TestHelper
    {
        public static Stream GetResourceStream(string name)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Concordia.Framework.Tests." + name);
        }

        public static Mock<T> SetupMock<T>() where T : class
        {
            var mock = new Mock<T>(MockBehavior.Strict);
            RegisterMockInDependencyResolver(mock);

            return mock;
        }

        public static void SetEntityEvict(Artist entity, bool value)
        {
            entity.GetType().GetProperty("Evicted", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(entity, value);
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
            entity.PropertyChangeTracker.DisableChangeTracking = true;
            entity.GetType().GetProperty("Id").SetValue(entity, id);
            entity.GetType().GetProperty("Version").SetValue(entity, 1);
            entity.PropertyChangeTracker.DisableChangeTracking = false;

            return entity;
        }

        public static Mock<Session> CreateSessionMock()
        {
            var connectionMock = SetupMock<IConnection>();
            connectionMock.Setup(s => s.Open());

            var session = SetupMock<Session>(connectionMock.Object);

            return session;
        }

        public static Mock<IConnection> SetupConnection()
        {
            var connectionMock = SetupMock<IConnection>();
            connectionMock.Setup(s => s.Open());

            return connectionMock;
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
            dataReader.Setup(s => s["Id"]).Returns(1);
            dataReader.Setup(s => s["Version"]).Returns(1);
            dataReader.Setup(s => s["Deleted"]).Returns(false);
        }

        public static void SetupEntityMetadataServices()
        {
            DependencyResolver.Register<IEntityMetadataService>(new EntityMetadataService());
            DependencyResolver.Register<IEntityMetadataResolver>(new EntityMetadataResolver());
        }

        public static Mock<ISqlTokenComposerService> SetupSqlTokenComposer()
        {
            var composerMock = SetupMock<ISqlTokenComposerService>();
            composerMock.Setup(s => s.ComposeSql(It.IsAny<IEnumerable<SqlToken>>())).Returns(new TokenComposerResult(string.Empty, Enumerable.Empty<QueryParameter>()));

            return composerMock;
        }
    }
}

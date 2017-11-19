using System;
using System.Data;
using System.IO;
using System.Reflection;
using Moq;
using Nightingale.Entities;
using Nightingale.Metadata;
using Nightingale.Tests.DataSources;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Nightingale.Tests
{
    internal static class TestHelper
    {
        public static Stream GetResourceStream(string name)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Nightingale.Tests." + name);
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
    }
}

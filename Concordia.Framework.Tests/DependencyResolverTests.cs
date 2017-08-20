using System;
using Xunit;

namespace Concordia.Framework.Tests
{
    public class DependencyResolverTests
    {
        public DependencyResolverTests()
        {
            DependencyResolver.Clear();
        }
    
        [Fact]
        public void Register_Instance_Created_When_Only_Type_Supplied()
        {
            // act
            DependencyResolver.Register<EntityService>();

            // assert
            Assert.NotNull(DependencyResolver.GetInstance<EntityService>());
        }

        [Fact]
        public void Register_Instance_Can_Be_Retrieved()
        {
            // act
            DependencyResolver.Register<IEntityService>(new EntityService());

            // assert
            Assert.Equal(typeof(EntityService), DependencyResolver.GetInstance<IEntityService>().GetType());
        }

        [Fact]
        public void Clear_Removes_All_Registered_Instances()
        {
            // arrange
            DependencyResolver.Register<IEntityService>(new EntityService());

            // act
            DependencyResolver.Clear();

            // assert
            Assert.Throws<InvalidOperationException>(() => DependencyResolver.GetInstance<IEntityService>());
        }
    }
}

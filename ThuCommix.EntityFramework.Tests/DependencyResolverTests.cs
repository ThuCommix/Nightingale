using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ThuCommix.EntityFramework.Tests
{
    [TestFixture]
    public class DependencyResolverTests
    {
        [SetUp]
        public void Setup()
        {
            DependencyResolver.Clear();
        }
    
        [Test]
        public void Register_Instance_Created_When_Only_Type_Supplied()
        {
            // act
            DependencyResolver.Register<EntityService>();

            // assert
            Assert.That(DependencyResolver.GetInstance<EntityService>(), Is.Not.Null);
        }

        [Test]
        public void Register_Instance_Can_Be_Retrieved()
        {
            // act
            DependencyResolver.Register<IEntityService>(new EntityService());

            // assert
            Assert.That(DependencyResolver.GetInstance<IEntityService>().GetType(), Is.EqualTo(typeof(EntityService)));
        }

        [Test]
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

using System.Linq;
using NUnit.Framework;
using Concordia.Framework.Metadata;
using Concordia.Framework.Tests.DataSources;

namespace Concordia.Framework.Tests
{
    [TestFixture]
    public class PropertyChangeTrackerTests
    {
        [SetUp]
        public void Setup()
        {
            DependencyResolver.Clear();
            DependencyResolver.Register<IEntityMetadataService>(new EntityMetadataService());
            DependencyResolver.Register<IEntityMetadataResolver>(new EntityMetadataResolver());
        }

        [Test]
        public void HasChanged_Should_Not_Track_Property_Changes_With_The_Same_NewValue_As_The_Old_Value()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var propertyChangeTracker = new PropertyChangeTracker(entity);

            propertyChangeTracker.AddPropertyChangedItem<Artist>(x => x.Name, "TestValue", "TestValue");

            // act
            var result = propertyChangeTracker.HasChanged<Artist>(x => x.Name);

            // assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void HasChanged_Works()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var propertyChangeTracker = new PropertyChangeTracker(entity);

            propertyChangeTracker.AddPropertyChangedItem<Artist>(x => x.Name, "OldValue", "NewValue");

            // act
            var result = propertyChangeTracker.HasChanged<Artist>(x => x.Name);

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasChanged_Returns_True_For_Unsaved_Entity()
        {
            // arrange
            var propertyChangeTracker = new PropertyChangeTracker(new Artist());

            // act
            var result = propertyChangeTracker.HasChanged<Artist>(x => x.Name);

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasChanged_Returns_True_For_Newly_Created_Entity_ForeignKey()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var propertyChangeTracker = new PropertyChangeTracker(entity);

            propertyChangeTracker.AddPropertyChangedItem<Artist>(x => x.FK_AnotherArtist_ID, 0, 0);

            // act
            var result = propertyChangeTracker.HasChanged<Artist>(x => x.FK_AnotherArtist_ID);

            // assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void TryGetReplacedValue_Extracts_The_Oldest_Value()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var propertyChangeTracker = new PropertyChangeTracker(entity);

            propertyChangeTracker.AddPropertyChangedItem<Artist>(x => x.Name, "T1", "T2");
            propertyChangeTracker.AddPropertyChangedItem<Artist>(x => x.Name, "T2", "T3");

            // act
            string value;
            propertyChangeTracker.TryGetReplacedValue<Artist, string>(x => x.Name, out value);

            // assert
            Assert.That(value, Is.EqualTo("T1"));
        }

        [Test]
        public void AddPropertyChangedItem_Does_Nothing_When_Tracking_Is_Disabled()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var propertyChangeTracker = new PropertyChangeTracker(entity);

            propertyChangeTracker.DisableChangeTracking = true;
            propertyChangeTracker.AddPropertyChangedItem<Artist>(x => x.Name, "OldValue", "NewValue");

            // act
            var result = propertyChangeTracker.HasChanged<Artist>(x => x.Name);

            // asset
            Assert.That(result, Is.False);
        }

        [Test]
        public void Clear_Any_Changes_Are_Discarded()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var propertyChangeTracker = new PropertyChangeTracker(entity);

            propertyChangeTracker.AddPropertyChangedItem<Artist>(x => x.Name, "OldValue", "NewValue");

            // act
            propertyChangeTracker.Clear();

            // asset
            Assert.That(propertyChangeTracker.HasChanges, Is.False);
        }

        [Test]
        public void GetChangedProperties_Works()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var propertyChangeTracker = new PropertyChangeTracker(entity);

            propertyChangeTracker.AddPropertyChangedItem<Artist>(x => x.Name, "OldValue", "NewValue");

            // act
            var result = propertyChangeTracker.GetChangedProperties();

            // asset
            Assert.That(result.First(), Is.EqualTo("Name"));
        }
    }
}

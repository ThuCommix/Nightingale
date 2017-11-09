using System.Collections.Generic;
using System.Linq;
using Nightingale.Metadata;
using Nightingale.Tests.DataSources;
using Xunit;

namespace Nightingale.Tests
{
    public class PropertyChangeTrackerTests
    {
        public PropertyChangeTrackerTests()
        {
            DependencyResolver.Clear();
            DependencyResolver.Register<IEntityMetadataService>(new EntityMetadataService());
            DependencyResolver.Register<IEntityMetadataResolver>(new EntityMetadataResolver());
        }

        [Fact]
        public void HasChanged_Should_Not_Track_Property_Changes_With_The_Same_NewValue_As_The_Old_Value()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var propertyChangeTracker = new PropertyChangeTracker(entity);

            propertyChangeTracker.AddPropertyChangedItem<Artist>(x => x.Name, "TestValue", "TestValue");

            // act
            var result = propertyChangeTracker.HasChanged<Artist>(x => x.Name);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void HasChanged_Works()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var propertyChangeTracker = new PropertyChangeTracker(entity);

            propertyChangeTracker.AddPropertyChangedItem<Artist>(x => x.Name, "OldValue", "NewValue");

            // act
            var result = propertyChangeTracker.HasChanged<Artist>(x => x.Name);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void HasChanged_Returns_True_For_Unsaved_Entity()
        {
            // arrange
            var propertyChangeTracker = new PropertyChangeTracker(new Artist());

            // act
            var result = propertyChangeTracker.HasChanged<Artist>(x => x.Name);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void HasChanged_Returns_True_For_Newly_Created_Entity_ForeignKey()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var propertyChangeTracker = new PropertyChangeTracker(entity);

            propertyChangeTracker.AddPropertyChangedItem<Artist>(x => x.FK_AnotherArtist_ID, 0, 0);

            // act
            var result = propertyChangeTracker.HasChanged<Artist>(x => x.FK_AnotherArtist_ID);

            // assert
            Assert.True(result);
        }

        [Fact]
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
            Assert.Equal("T1", value);
        }

        [Fact]
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
            Assert.False(result);
        }

        [Fact]
        public void Clear_Any_Changes_Are_Discarded()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var propertyChangeTracker = new PropertyChangeTracker(entity);

            propertyChangeTracker.AddPropertyChangedItem<Artist>(x => x.Name, "OldValue", "NewValue");

            // act
            propertyChangeTracker.Clear();

            // asset
            Assert.False(propertyChangeTracker.HasChanges);
        }

        [Fact]
        public void GetChangedProperties_Works()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var propertyChangeTracker = new PropertyChangeTracker(entity);

            propertyChangeTracker.AddPropertyChangedItem<Artist>(x => x.Name, "OldValue", "NewValue");

            // act
            var result = propertyChangeTracker.GetChangedProperties();

            // asset
            Assert.Equal("Name", result.First());
        }

        [Theory]
        [InlineData(CollectionChangeType.Added)]
        [InlineData(CollectionChangeType.Removed)]
        public void AddCollectionChangedItem_Adds_ChangedItem(CollectionChangeType changeType)
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var propertyChangeTracker = new PropertyChangeTracker(entity);

            // act
            propertyChangeTracker.AddCollectionChangedItem(new CollectionChangedItem("StatisticValues", null, changeType));

            // assert
            Assert.True(propertyChangeTracker.HasChanged<Artist>(x => x.StatisticValues));
        }

        [Theory]
        [InlineData(CollectionChangeType.Added)]
        [InlineData(CollectionChangeType.Removed)]
        public void AddCollectionChangedItem_Prevents_Unneccessary_Changes(CollectionChangeType changeType)
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var propertyChangeTracker = new PropertyChangeTracker(entity);
            var oppositeChangeType = changeType == CollectionChangeType.Added ? CollectionChangeType.Removed : CollectionChangeType.Added;
            var statisticValue = new ArtistStatisticValues();

            // act
            propertyChangeTracker.AddCollectionChangedItem(new CollectionChangedItem("StatisticValues", statisticValue, changeType));
            propertyChangeTracker.AddCollectionChangedItem(new CollectionChangedItem("StatisticValues", statisticValue, oppositeChangeType));

            // assert
            Assert.False(propertyChangeTracker.HasChanged<Artist>(x => x.StatisticValues));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AddCollectionChangedItem_Does_Nothing_When_Tracking_Is_Disabled(bool disableChangeTracking)
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var propertyChangeTracker = new PropertyChangeTracker(entity) { DisableChangeTracking = disableChangeTracking };

            // act
            propertyChangeTracker.AddCollectionChangedItem(new CollectionChangedItem("StatisticValues", null, CollectionChangeType.Added));

            // assert
            Assert.Equal(!disableChangeTracking, propertyChangeTracker.HasChanged<Artist>(x => x.StatisticValues));
        }

        [Fact]
        public void TryGetAddedCollectionItems_Can_Get_Added_Items()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist", "StatisticValues");

            entityCollection.Add(new ArtistStatisticValues());
            entityCollection.Add(new ArtistStatisticValues());

            List<ArtistStatisticValues> listAddedItems;

            // act
            var result = entity.PropertyChangeTracker.TryGetAddedCollectionItems<Artist, ArtistStatisticValues>(x => x.StatisticValues, out listAddedItems);

            // assert
            Assert.True(result);
            Assert.NotNull(listAddedItems);
            Assert.Equal(2, listAddedItems.Count);
            Assert.Equal(entityCollection[0], listAddedItems[0]);
            Assert.Equal(entityCollection[1], listAddedItems[1]);
        }

        [Fact]
        public void TryGetRemovedCollectionItems_Can_Get_Removed_Items()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist", "StatisticValues");

            var statisticValues = new ArtistStatisticValues();

            entity.PropertyChangeTracker.DisableChangeTracking = true;
            entityCollection.Add(statisticValues);
            entity.PropertyChangeTracker.DisableChangeTracking = false;

            entityCollection.Remove(statisticValues);

            List<ArtistStatisticValues> listRemovedItems;

            // act
            var result = entity.PropertyChangeTracker.TryGetRemovedCollectionItems<Artist, ArtistStatisticValues>(x => x.StatisticValues, out listRemovedItems);

            // assert
            Assert.True(result);
            Assert.NotNull(listRemovedItems);
            Assert.Equal(statisticValues, listRemovedItems[0]);
        }
    }
}

using Concordia.Framework.Tests.DataSources;
using Xunit;

namespace Concordia.Framework.Tests
{
    public class EntityCollectionTests
    {
        public EntityCollectionTests()
        {
            DependencyResolver.Clear();
        }

        [Fact]
        public void Add_Sets_Foreign_Field()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var artistStatisticValues = new ArtistStatisticValues();

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist", "StatisticValues");

            // act
            entityCollection.Add(artistStatisticValues);

            // assert
            Assert.Equal(artistStatisticValues.AnotherArtist, entity);
        }

        [Fact]
        public void AddRange_Sets_Foreign_Field()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var artistStatisticValues = new ArtistStatisticValues();

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist", "StatisticValues");

            // act
            entityCollection.AddRange(new[] { artistStatisticValues });

            // assert
            Assert.Equal(artistStatisticValues.AnotherArtist, entity);
        }

        [Fact]
        public void Insert_Sets_Foreign_Field()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var artistStatisticValues = new ArtistStatisticValues();

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist", "StatisticValues");

            // act
            entityCollection.Insert(0, artistStatisticValues);

            // assert
            Assert.Equal(artistStatisticValues.AnotherArtist, entity);
        }

        [Fact]
        public void CopyTo_Sets_Foreign_Field()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var artistStatisticValues = new ArtistStatisticValues();

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist", "StatisticValues");

            // act
            entityCollection.CopyTo(new[] { artistStatisticValues }, 0);

            // assert
            Assert.Equal(artistStatisticValues.AnotherArtist, entity);
        }

        [Fact]
        public void Remove_Removes_Foreign_Field()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var artistStatisticValues = new ArtistStatisticValues();

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "SecondArtist", "StatisticValues");
            entityCollection.Add(artistStatisticValues);

            TestHelper.SetupEntityMetadataServices();

            // act
            entityCollection.Remove(artistStatisticValues);

            // assert
            Assert.Null(artistStatisticValues.SecondArtist);
        }

        [Fact]
        public void RemoveAt_Removes_Foreign_Field()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var artistStatisticValues = new ArtistStatisticValues();

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "SecondArtist", "StatisticValues");
            entityCollection.Add(artistStatisticValues);

            TestHelper.SetupEntityMetadataServices();

            // act
            entityCollection.RemoveAt(0);

            // assert
            Assert.Null(artistStatisticValues.SecondArtist);
        }

        [Fact]
        public void RemoveAt_Removes_Not_When_ReferenceField_Is_Mandatory()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var artistStatisticValues = new ArtistStatisticValues();

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist", "StatisticValues");
            entityCollection.Add(artistStatisticValues);

            TestHelper.SetupEntityMetadataServices();

            // act
            entityCollection.RemoveAt(0);

            // assert
            Assert.NotNull(artistStatisticValues.AnotherArtist);
        }

        [Fact]
        public void Remove_Removes_Not_When_ReferenceField_Is_Mandatory()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var artistStatisticValues = new ArtistStatisticValues();

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist", "StatisticValues");
            entityCollection.Add(artistStatisticValues);

            TestHelper.SetupEntityMetadataServices();

            // act
            entityCollection.Remove(artistStatisticValues);

            // assert
            Assert.NotNull(artistStatisticValues.AnotherArtist);
        }

        [Fact]
        public void Add_Adds_Collection_Changed_Item()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var artistStatisticValues = new ArtistStatisticValues();

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist", "StatisticValues");

            TestHelper.SetupEntityMetadataServices();

            // act
            entityCollection.Add(artistStatisticValues);

            // assert
            Assert.True(entity.PropertyChangeTracker.HasChanged<Artist>(x => x.StatisticValues));
        }

        [Fact]
        public void Remove_Adds_Collection_Changed_Item()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var artistStatisticValues = new ArtistStatisticValues();

            entity.PropertyChangeTracker.DisableChangeTracking = true;
            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist", "StatisticValues");
            entityCollection.Add(artistStatisticValues);
            entity.PropertyChangeTracker.DisableChangeTracking = false;

            TestHelper.SetupEntityMetadataServices();

            // act
            entityCollection.Remove(artistStatisticValues);

            // assert
            Assert.True(entity.PropertyChangeTracker.HasChanged<Artist>(x => x.StatisticValues));
        }
    }
}

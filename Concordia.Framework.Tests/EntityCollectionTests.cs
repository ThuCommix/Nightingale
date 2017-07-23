using NUnit.Framework;
using Concordia.Framework.Tests.DataSources;

namespace Concordia.Framework.Tests
{
    [TestFixture]
    public class EntityCollectionTests
    {
        [SetUp]
        public void Setup()
        {
            DependencyResolver.Clear();
        }

        [Test]
        public void Add_Sets_Foreign_Field()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var artistStatisticValues = new ArtistStatisticValues();

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist", "StatisticValues");

            // act
            entityCollection.Add(artistStatisticValues);

            // assert
            Assert.That(artistStatisticValues.AnotherArtist, Is.EqualTo(entity));
        }

        [Test]
        public void AddRange_Sets_Foreign_Field()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var artistStatisticValues = new ArtistStatisticValues();

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist", "StatisticValues");

            // act
            entityCollection.AddRange(new[] { artistStatisticValues });

            // assert
            Assert.That(artistStatisticValues.AnotherArtist, Is.EqualTo(entity));
        }

        [Test]
        public void Insert_Sets_Foreign_Field()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var artistStatisticValues = new ArtistStatisticValues();

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist", "StatisticValues");

            // act
            entityCollection.Insert(0, artistStatisticValues);

            // assert
            Assert.That(artistStatisticValues.AnotherArtist, Is.EqualTo(entity));
        }

        [Test]
        public void CopyTo_Sets_Foreign_Field()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            var artistStatisticValues = new ArtistStatisticValues();

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist", "StatisticValues");

            // act
            entityCollection.CopyTo(new[] { artistStatisticValues }, 0);

            // assert
            Assert.That(artistStatisticValues.AnotherArtist, Is.EqualTo(entity));
        }

        [Test]
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
            Assert.That(artistStatisticValues.SecondArtist, Is.Null);
        }

        [Test]
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
            Assert.That(artistStatisticValues.SecondArtist, Is.Null);
        }

        [Test]
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
            Assert.That(artistStatisticValues.AnotherArtist, Is.Not.Null);
        }

        [Test]
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
            Assert.That(artistStatisticValues.AnotherArtist, Is.Not.Null);
        }

        [Test]
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
            Assert.That(entity.PropertyChangeTracker.HasChanged<Artist>(x => x.StatisticValues), Is.True);
        }

        [Test]
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
            Assert.That(entity.PropertyChangeTracker.HasChanged<Artist>(x => x.StatisticValues), Is.True);
        }
    }
}

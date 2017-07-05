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

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist");

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

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist");

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

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist");

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

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist");

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

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "SecondArtist");
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

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "SecondArtist");
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

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist");
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

            var entityCollection = new EntityCollection<ArtistStatisticValues>(entity, "AnotherArtist");
            entityCollection.Add(artistStatisticValues);

            TestHelper.SetupEntityMetadataServices();

            // act
            entityCollection.Remove(artistStatisticValues);

            // assert
            Assert.That(artistStatisticValues.AnotherArtist, Is.Not.Null);
        }
    }
}

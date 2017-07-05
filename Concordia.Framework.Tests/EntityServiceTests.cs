using System;
using System.Data;
using System.Linq;
using Moq;
using NUnit.Framework;
using Concordia.Framework.Entities;
using Concordia.Framework.Tests.DataSources;

namespace Concordia.Framework.Tests
{
    [TestFixture]
    public class EntityServiceTests
    {
        [SetUp]
        public void Setup()
        {
            DependencyResolver.Clear();
            TestHelper.SetupEntityMetadataServices();
        }

        [Test]
        public void GetChildEntities_Returns_Expected_Entities_With_Cascade_Save()
        {
            // arrange
            var artist = TestHelper.CreateEntityWithId<Artist>(1);
            var statisticValue = new ArtistStatisticValues();
            var anotherArtist = new Artist();

            artist.AnotherArtist = anotherArtist;
            artist.StatisticValues.Add(statisticValue);

            // act
            var result = new EntityService().GetChildEntities(artist, Cascade.Save);

            // assert
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result.Any(x => x == artist), Is.True);
            Assert.That(result.Any(x => x == statisticValue), Is.True);
            Assert.That(result.Any(x => x == anotherArtist), Is.True);
        }

        [Test]
        public void GetChildEntities_Returns_Expected_Entities_With_Cascade_SaveDelete()
        {
            // arrange
            var artist = TestHelper.CreateEntityWithId<Artist>(1);
            var statisticValue = new ArtistStatisticValues();
            var anotherArtist = new Artist();

            artist.AnotherArtist = anotherArtist;
            artist.StatisticValues.Add(statisticValue);

            // act
            var result = new EntityService().GetChildEntities(artist, Cascade.SaveDelete);

            // assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.Any(x => x == artist), Is.True);
        }

        [Test]
        public void UpdateForeignFields_Synchronizes_Int_Field_With_Id_Of_FK()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            entity.AnotherArtist = TestHelper.CreateEntityWithId<Artist>(2);

            // act
            new EntityService().UpdateForeignFields(entity);

            // assert
            Assert.That(entity.FK_AnotherArtist_ID, Is.EqualTo(entity.AnotherArtist.Id));
        }

        [Test]
        public void UpdateForeignFields_Synchronizes_Id_With_The_ReferenceField_Of_A_List_Item()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            entity.StatisticValues.Add(new ArtistStatisticValues());

            // act
            new EntityService().UpdateForeignFields(entity);

            // assert
            Assert.That(entity.StatisticValues[0].FK_AnotherArtist_ID, Is.EqualTo(entity.Id));
        }

        [Test]
        public void CreateEntity_Throw_When_Reader_Null()
        {
            // act
            Assert.Throws<ArgumentNullException>(() => new EntityService().CreateEntity(null, typeof(Artist)));
        }

        [Test]
        public void CreateEntity_Throw_When_EntityType_Null()
        {
            // arrange
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();

            // act
            Assert.Throws<ArgumentNullException>(() => new EntityService().CreateEntity(null, null));

            // assert
            dataReaderMock.VerifyAll();
        }

        [Test]
        public void CreateEntity_PropertyChangeTracker_Is_Enabled_After_Creation()
        {
            // arrange
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();
            dataReaderMock.Setup(s => s[It.IsAny<string>()]).Returns((object)null);

            TestHelper.SetupEntityMetadataServices();

            // act
            var result = new EntityService().CreateEntity(dataReaderMock.Object, typeof(Artist));

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.GetType(), Is.EqualTo(typeof(Artist)));
            Assert.That(result.PropertyChangeTracker.DisableChangeTracking, Is.False);

            dataReaderMock.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateEntity_Continue_When_DBNull_Or_Null(bool dbNullOrNull)
        {
            // arrange
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();
            dataReaderMock.Setup(s => s[It.IsAny<string>()]).Returns(dbNullOrNull ? DBNull.Value : null);

            TestHelper.SetupEntityMetadataServices();

            // act
            var result = new EntityService().CreateEntity(dataReaderMock.Object, typeof(Artist)) as Artist;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(0));
            Assert.That(result.Alias, Is.Null);
            Assert.That(result.Name, Is.Null);
            Assert.That(result.Note, Is.Null);
            Assert.That(result.Label, Is.Null);
            Assert.That(result.Biography, Is.Null);
            Assert.That(result.BirthDate, Is.Null);
            Assert.That(result.DeathDate, Is.Null);
            Assert.That(result.AnotherArtist, Is.Null);

            dataReaderMock.VerifyAll();
        }

        [Test]
        public void CreateEntity_Fill_Expected_Fields()
        {
            // arrange
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();

            TestHelper.SetupDataReaderEntityBaseProperties(dataReaderMock);

            dataReaderMock.Setup(s => s["FK_AnotherArtist_ID"]).Returns(1);
            dataReaderMock.Setup(s => s["FK_SecondArtist_ID"]).Returns((object)null);
            dataReaderMock.Setup(s => s["StatusCode"]).Returns(0);

            TestHelper.SetupEntityMetadataServices();

            // act
            var result = new EntityService().CreateEntity(dataReaderMock.Object, typeof(ArtistStatisticValues)) as ArtistStatisticValues;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Version, Is.EqualTo(1));
            Assert.That(result.Deleted, Is.EqualTo(false));
            Assert.That(result.FK_AnotherArtist_ID, Is.EqualTo(1));
            Assert.That(result.FK_SecondArtist_ID, Is.Null);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCode.None));
        }
    }
}

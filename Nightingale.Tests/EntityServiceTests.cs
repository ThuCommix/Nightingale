using System;
using System.Data;
using Moq;
using Nightingale.Entities;
using Nightingale.Tests.DataSources;
using Xunit;

namespace Nightingale.Tests
{
    public class EntityServiceTests
    {
        public EntityServiceTests()
        {
            DependencyResolver.Clear();
            TestHelper.SetupEntityMetadataServices();
        }

        [Fact]
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
            Assert.Equal(3, result.Count);
            Assert.Contains(result, x => x == artist);
            Assert.Contains(result, x => x == statisticValue);
            Assert.Contains(result, x => x == anotherArtist);
        }

        [Fact]
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
            Assert.Single(result);
            Assert.Contains(result, x => x == artist);
        }

        [Fact]
        public void UpdateForeignFields_Synchronizes_Int_Field_With_Id_Of_FK()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            entity.AnotherArtist = TestHelper.CreateEntityWithId<Artist>(2);

            // act
            new EntityService().UpdateForeignFields(entity);

            // assert
            Assert.Equal(entity.AnotherArtist.Id, entity.FK_AnotherArtist_ID);
        }

        [Fact]
        public void UpdateForeignFields_Synchronizes_Id_With_The_ReferenceField_Of_A_List_Item()
        {
            // arrange
            var entity = TestHelper.CreateEntityWithId<Artist>(1);
            entity.StatisticValues.Add(new ArtistStatisticValues());

            // act
            new EntityService().UpdateForeignFields(entity);

            // assert
            Assert.Equal(entity.Id, entity.StatisticValues[0].FK_AnotherArtist_ID);
        }

        [Fact]
        public void CreateEntity_Throw_When_Reader_Null()
        {
            // act
            Assert.Throws<ArgumentNullException>(() => new EntityService().CreateEntity(null, typeof(Artist)));
        }

        [Fact]
        public void CreateEntity_Throw_When_EntityType_Null()
        {
            // arrange
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();

            // act
            Assert.Throws<ArgumentNullException>(() => new EntityService().CreateEntity(null, null));

            // assert
            dataReaderMock.VerifyAll();
        }

        [Fact]
        public void CreateEntity_PropertyChangeTracker_Is_Enabled_After_Creation()
        {
            // arrange
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();
            dataReaderMock.Setup(s => s[It.IsAny<string>()]).Returns((object)null);

            TestHelper.SetupEntityMetadataServices();

            // act
            var result = new EntityService().CreateEntity(dataReaderMock.Object, typeof(Artist));

            // assert
            Assert.NotNull(result);
            Assert.Equal(typeof(Artist), result.GetType());
            Assert.False(result.PropertyChangeTracker.DisableChangeTracking);

            dataReaderMock.VerifyAll();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CreateEntity_Continue_When_DBNull_Or_Null(bool dbNullOrNull)
        {
            // arrange
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();
            dataReaderMock.Setup(s => s[It.IsAny<string>()]).Returns(dbNullOrNull ? DBNull.Value : null);

            TestHelper.SetupEntityMetadataServices();

            // act
            var result = new EntityService().CreateEntity(dataReaderMock.Object, typeof(Artist)) as Artist;

            // assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Id);
            Assert.Null(result.Alias);
            Assert.Null(result.Name);
            Assert.Null(result.Note);
            Assert.Null(result.Label);
            Assert.Null(result.Biography);
            Assert.Null(result.BirthDate);
            Assert.Null(result.DeathDate);
            Assert.Null(result.AnotherArtist);

            dataReaderMock.VerifyAll();
        }

        [Fact]
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
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(1, result.Version);
            Assert.False(result.Deleted);
            Assert.Equal(1, result.FK_AnotherArtist_ID);
            Assert.Null(result.FK_SecondArtist_ID);
            Assert.Equal(StatusCode.None, result.StatusCode);
        }
    }
}

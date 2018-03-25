using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Moq;
using Nightingale.Entities;
using Nightingale.Metadata;
using Nightingale.Queries;
using Nightingale.Sessions;
using Nightingale.Tests.DataSources;
using Xunit;

namespace Nightingale.Tests.Queries
{
    public class QueryableTests
    {
        public QueryableTests()
        {
            DependencyResolver.Clear();
            TestHelper.SetupEntityMetadataServices();
        }

        [Fact]
        public void Queryable_Equals_Null()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.Name == null).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_Equals_Null, queryResult.Command);
        }

        [Fact]
        public void Queryable_Equals_Not_Null()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.Name != null).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_Equals_Not_Null, queryResult.Command);
        }

        [Fact]
        public void Queryable_Equals()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.Name == string.Empty).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_Equals, queryResult.Command);
        }

        [Fact]
        public void Queryable_Not_Equals()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.Name != string.Empty).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_Not_Equals, queryResult.Command);
        }

        [Fact]
        public void Queryable_GreaterThan()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.Id > 1).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_GreaterThan, queryResult.Command);
        }

        [Fact]
        public void Queryable_GreaterThan_Or_Equals()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.Id >= 1).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_GreaterThan_Or_Equals, queryResult.Command);
        }

        [Fact]
        public void Queryable_LowerThan()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.Id < 1).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_LowerThan, queryResult.Command);
        }

        [Fact]
        public void Queryable_LowerThan_Or_Equals()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.Id <= 1).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_LowerThan_Or_Equals, queryResult.Command);
        }

        [Fact]
        public void Queryable_Multiple_And_Conditions()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.Name == string.Empty && x.Deleted == false).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_Multiple_And_Conditions, queryResult.Command);
        }

        [Fact]
        public void Queryable_Multiple_Or_Conditions()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.Name == string.Empty || x.Deleted == false).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_Multiple_Or_Conditions, queryResult.Command);
        }

        [Fact]
        public void Queryable_Mixed_And_Or_Conditions()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.Name == string.Empty && x.Alias == string.Empty || x.Deleted == false).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_Mixed_And_Or_Conditions, queryResult.Command);
        }

        [Fact]
        public void Queryable_With_Joins()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.AnotherArtist.Name == string.Empty && x.AnotherArtist.AnotherArtist.Alias == string.Empty).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_With_Joins, queryResult.Command);
        }

        [Fact]
        public void Queryable_NonScalar_Property_Check_Results_In_Fk_Check()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.AnotherArtist == null).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_NonScalar_Property_Check_Results_In_Fk_Check, queryResult.Command);
        }

        [Fact]
        public void Queryable_Take()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.AnotherArtist == null).Take(10).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_Take, queryResult.Command);
        }

        [Fact]
        public void Queryable_Skip()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.AnotherArtist == null).Skip(10).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_Skip, queryResult.Command);
        }

        [Fact]
        public void Queryable_SkipTake()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.AnotherArtist == null).Skip(10).Take(10).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_SkipTake, queryResult.Command);
        }

        [Fact]
        public void Queryable_Contains()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.Name.Contains("abc")).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_Contains, queryResult.Command);
            Assert.Equal("%abc%", queryResult.Parameters.First().Value);
        }

        [Fact]
        public void Queryable_StartsWith()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.Name.StartsWith("abc")).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_Contains, queryResult.Command);
            Assert.Equal("abc%", queryResult.Parameters.First().Value);
        }

        [Fact]
        public void Queryable_EndsWith()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.Name.EndsWith("abc")).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_Contains, queryResult.Command);
            Assert.Equal("%abc", queryResult.Parameters.First().Value);
        }

        [Fact]
        public void Queryable_OrderBy()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.OrderBy(x => x.Name).ThenBy(x => x.Alias).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_OrderBy, queryResult.Command);
        }

        [Fact]
        public void Queryable_OrderByDescending()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.OrderByDescending(x => x.Name).ThenByDescending(x => x.Alias).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_OrderByDescending, queryResult.Command);
        }

        [Fact]
        public void Queryable_Mixed_Order()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.OrderByDescending(x => x.Name).ThenBy(x => x.Alias).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_Mixed_Order, queryResult.Command);
        }

        [Fact]
        public void Queryable_FirstOrDefault()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.FirstOrDefault(x => x.Name != string.Empty);

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_FirstOrDefault, queryResult.Command);
        }

        [Fact]
        public void Queryable_First_Throws_When_Sequence_Empty()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            Assert.Throws<InvalidOperationException>(() => queryable.First(x => x.Name != string.Empty));
        }

        [Fact]
        public void Queryable_Single_Throws_When_Sequence_Empty()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>()).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            Assert.Throws<InvalidOperationException>(() => queryable.Single(x => x.Name != string.Empty));
        }

        [Fact]
        public void Queryable_Single_Throws_When_Sequence_Has_More_Than_One()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>{new Artist(), new Artist()}).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            Assert.Throws<InvalidOperationException>(() => queryable.Single(x => x.Name != string.Empty));
        }

        [Fact]
        public void Queryable_SingleOrDefault_Throws_When_Sequence_Has_More_Than_One()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity> { new Artist(), new Artist() }).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            Assert.Throws<InvalidOperationException>(() => queryable.Single(x => x.Name != string.Empty));
        }

        [Fact]
        public void Queryable_First()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity>{new Artist()}).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.First(x => x.Name != string.Empty);

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_FirstOrDefault, queryResult.Command);
        }

        [Fact]
        public void Queryable_Any()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity> { new Artist() }).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.StatisticValues.Any(y => y.AnotherArtist.Name == string.Empty)).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_Any, queryResult.Command);
        }

        [Fact]
        public void Queryable_All()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity> { new Artist() }).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Artist>(session.Object, null);

            // act
            var result = queryable.Where(x => x.StatisticValues.All(y => y.AnotherArtist.Name == string.Empty)).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_All, queryResult.Command);
        }

        [Fact]
        public void Queryable_Multiple_Where_Resets_EntityMetadata()
        {
            // arrange
            IQuery queryResult = null;
            var session = TestHelper.SetupMock<ISession>();
            session.Setup(s => s.ExecuteQuery(It.IsAny<IQuery>())).Returns(new List<Entity> { new Song() }).Callback<IQuery>(q => queryResult = q);

            var queryable = new Queryable<Song>(session.Object, null);

            // act
            var result = queryable.Where(x => x.Artist.Name != null).Where(x => x.Title != null).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_Multiple_Where_Resets_EntityMetadata, queryResult.Command);
        }

        [Fact]
        public void Queryable_Select_Support_Dynamic_Objects()
        {
            // arrange
            IQuery queryResult = null;

            var canRead = true;
            var session = TestHelper.SetupMock<ISession>();
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();
            dataReaderMock.Setup(s => s.Read()).Returns(() =>
            {
                if (!canRead)
                    return false;

                canRead = false;
                return true;
            });

            dataReaderMock.Setup(s => s.Dispose());
            dataReaderMock.Setup(s => s["e_Id"]).Returns(1);
            dataReaderMock.Setup(s => s["e_Title"]).Returns("Title");

            var connectionMock = TestHelper.SetupMock<IConnection>();
            connectionMock.Setup(s => s.ExecuteReader(It.IsAny<IQuery>())).Callback<IQuery>(q => queryResult = q).Returns(dataReaderMock.Object);

            session.Setup(s => s.Connection).Returns(connectionMock.Object);

            var queryable = new Queryable<Song>(session.Object, new EntityService());

            // act
            var result = queryable.Select(x => new { Id = x.Id, Text = x.Title }).ToList();

            // assert
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Title", result[0].Text);
            Assert.Equal(ExpectedQueryOutputs.Queryable_Select_Support_Dynamic_Objects, queryResult.Command);
        }

        [Fact]
        public void Queryable_Select_Support_Dynamic_Objects_With_Joins()
        {
            // arrange
            IQuery queryResult = null;

            var canRead = true;
            var session = TestHelper.SetupMock<ISession>();
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();
            dataReaderMock.Setup(s => s.Read()).Returns(() =>
            {
                if (!canRead)
                    return false;

                canRead = false;
                return true;
            });

            dataReaderMock.Setup(s => s.Dispose());
            dataReaderMock.Setup(s => s["e_Id"]).Returns(1);
            dataReaderMock.Setup(s => s["v1_Name"]).Returns("Title");

            var connectionMock = TestHelper.SetupMock<IConnection>();
            connectionMock.Setup(s => s.ExecuteReader(It.IsAny<IQuery>())).Callback<IQuery>(q => queryResult = q).Returns(dataReaderMock.Object);

            session.Setup(s => s.Connection).Returns(connectionMock.Object);

            var queryable = new Queryable<Song>(session.Object, new EntityService());

            // act
            var result = queryable.Select(x => new { Id = x.Id, Text = x.Artist.AnotherArtist.Name }).ToList();

            // assert
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Title", result[0].Text);
            Assert.Equal(ExpectedQueryOutputs.Queryable_Select_Support_Dynamic_Objects_With_Joins, queryResult.Command);
        }

        [Fact]
        public void Queryable_Select_Support_Dynamic_Objects_Full_Entity_Select()
        {
            // arrange
            IQuery queryResult = null;

            var canRead = true;
            var session = TestHelper.SetupMock<ISession>();
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();
            dataReaderMock.Setup(s => s.Read()).Returns(() =>
            {
                if (!canRead)
                    return false;

                canRead = false;
                return true;
            });

            dataReaderMock.Setup(s => s.Dispose());
            dataReaderMock.Setup(s => s["e_Id"]).Returns(1);

            var connectionMock = TestHelper.SetupMock<IConnection>();
            connectionMock.Setup(s => s.ExecuteReader(It.IsAny<IQuery>())).Callback<IQuery>(q => queryResult = q).Returns(dataReaderMock.Object);

            session.Setup(s => s.Connection).Returns(connectionMock.Object);

            var artist = new Artist();
            var entityServiceMock = TestHelper.SetupMock<IEntityService>();
            entityServiceMock.Setup(s => s.CreateEntity(dataReaderMock.Object, It.IsAny<EntityMetadata>(), "v1_")).Returns(artist);

            var queryable = new Queryable<Song>(session.Object, entityServiceMock.Object);

            // act
            var result = queryable.Select(x => new { Id = x.Id, AnotherArtist = x.Artist.AnotherArtist }).ToList();

            // assert
            Assert.Equal(1, result[0].Id);
            Assert.Equal(artist, result[0].AnotherArtist);
            Assert.Equal(ExpectedQueryOutputs.Queryable_Select_Support_Dynamic_Objects_Full_Entity_Select, queryResult.Command);
        }

        [Fact]
        public void Queryable_Select_Support_Dynamic_Objects_With_Joins_And_Where()
        {
            // arrange
            IQuery queryResult = null;

            var canRead = true;
            var session = TestHelper.SetupMock<ISession>();
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();
            dataReaderMock.Setup(s => s.Read()).Returns(() =>
            {
                if (!canRead)
                    return false;

                canRead = false;
                return true;
            });

            dataReaderMock.Setup(s => s.Dispose());
            dataReaderMock.Setup(s => s["e_Id"]).Returns(1);
            dataReaderMock.Setup(s => s["v1_Name"]).Returns("Title");

            var connectionMock = TestHelper.SetupMock<IConnection>();
            connectionMock.Setup(s => s.ExecuteReader(It.IsAny<IQuery>())).Callback<IQuery>(q => queryResult = q).Returns(dataReaderMock.Object);

            session.Setup(s => s.Connection).Returns(connectionMock.Object);

            var queryable = new Queryable<Song>(session.Object, new EntityService());

            // act
            var result = queryable.Where(x => x.Artist.AnotherArtist != null && x.Title != null).Select(x => new { Id = x.Id, Text = x.Artist.AnotherArtist.Name }).ToList();

            // assert
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Title", result[0].Text);
            Assert.Equal(ExpectedQueryOutputs.Queryable_Select_Support_Dynamic_Objects_With_Joins_And_Where, queryResult.Command);
        }

        [Fact]
        public void Queryable_Select_Member_Init()
        {
            // arrange
            IQuery queryResult = null;

            var canRead = true;
            var session = TestHelper.SetupMock<ISession>();
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();
            dataReaderMock.Setup(s => s.Read()).Returns(() =>
            {
                if (!canRead)
                    return false;

                canRead = false;
                return true;
            });

            dataReaderMock.Setup(s => s.Dispose());
            dataReaderMock.Setup(s => s["e_Id"]).Returns(1);
            dataReaderMock.Setup(s => s["e_Name"]).Returns("Title");

            var connectionMock = TestHelper.SetupMock<IConnection>();
            connectionMock.Setup(s => s.ExecuteReader(It.IsAny<IQuery>())).Callback<IQuery>(q => queryResult = q).Returns(dataReaderMock.Object);

            session.Setup(s => s.Connection).Returns(connectionMock.Object);

            var queryable = new Queryable<Artist>(session.Object, new EntityService());

            // act
            var result = queryable.Select(x => new CustomerModel { Id = x.Id, Name = x.Name }).ToList();

            // assert
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Title", result[0].Name);
            Assert.Equal(ExpectedQueryOutputs.Queryable_Select_Member_Init, queryResult.Command);
        }

        [Fact]
        public void Queryable_Select_Member_Init_And_Ctor()
        {
            // arrange
            IQuery queryResult = null;

            var canRead = true;
            var session = TestHelper.SetupMock<ISession>();
            var dataReaderMock = TestHelper.SetupMock<IDataReader>();
            dataReaderMock.Setup(s => s.Read()).Returns(() =>
            {
                if (!canRead)
                    return false;

                canRead = false;
                return true;
            });

            dataReaderMock.Setup(s => s.Dispose());
            dataReaderMock.Setup(s => s["e_Id"]).Returns(1);
            dataReaderMock.Setup(s => s["e_Name"]).Returns("Title");

            var connectionMock = TestHelper.SetupMock<IConnection>();
            connectionMock.Setup(s => s.ExecuteReader(It.IsAny<IQuery>())).Callback<IQuery>(q => queryResult = q).Returns(dataReaderMock.Object);

            session.Setup(s => s.Connection).Returns(connectionMock.Object);

            var queryable = new Queryable<Artist>(session.Object, new EntityService());

            // act
            var result = queryable.Select(x => new CustomerModel(x.Id) { Name = x.Name }).ToList();

            // assert
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Title", result[0].Name);
            Assert.Equal(ExpectedQueryOutputs.Queryable_Select_Member_Init_And_Ctor, queryResult.Command);
        }
    }
}

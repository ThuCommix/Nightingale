using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Nightingale.Entities;
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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

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

            var queryable = new Queryable<Artist>(session.Object);

            // act
            var result = queryable.Where(x => x.StatisticValues.All(y => y.AnotherArtist.Name == string.Empty)).ToList();

            // assert
            Assert.Equal(ExpectedQueryOutputs.Queryable_All, queryResult.Command);
        }
    }
}

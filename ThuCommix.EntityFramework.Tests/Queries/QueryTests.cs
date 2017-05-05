using NUnit.Framework;
using ThuCommix.EntityFramework.Queries;
using ThuCommix.EntityFramework.Tests.DataSources;

namespace ThuCommix.EntityFramework.Tests.Queries
{
    [TestFixture]
    public class QueryTests
    {
        [SetUp]
        public void Setup()
        {
            DependencyResolver.Clear();
            TestHelper.SetupEntityMetadataServices();
        }

        [Test]
        public void Query_Left_Join_For_Non_Mandatory_Fields_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.SecondArtist.Alias != null);

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_Left_Join_For_Non_Mandatory_Fields_Works));
        }

        [Test]
        public void Query_Inner_Join_For_Mandatory_Fields_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.AnotherArtist.Alias != null);

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_Inner_Join_For_Mandatory_Fields_Works));
        }

        [Test]
        public void Query_Null_Checks_On_Foreign_Properties_Do_Not_Create_Joins()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.SecondArtist != null);

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_Null_Checks_On_Foreign_Properties_Do_Not_Create_Joins));
        }
        
        [Test]
        public void Query_Operator_Equals_For_Null_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.SecondArtist == null);

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_Operator_Equals_For_Null_Works));
        }

        [Test]
        public void Query_Operator_Equals_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.Id == 1);

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_Operator_Equals_Works));
        }

        [Test]
        public void Query_Operator_NotEquals_For_Null_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.SecondArtist != null);

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_Operator_NotEquals_For_Null_Works));
        }

        [Test]
        public void Query_Operator_NotEquals_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.Id != 1);

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_Operator_NotEquals_Works));
        }

        [Test]
        public void Query_Operator_GreaterThan_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.Id > 1);

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_Operator_GreaterThan_Works));
        }

        [Test]
        public void Query_Operator_GreaterThanOrEquals_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.Id >= 1);

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_Operator_GreaterThanOrEquals_Works));
        }

        [Test]
        public void Query_Operator_LessThan_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.Id < 1);

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_Operator_LessThan_Works));
        }

        [Test]
        public void Query_Operator_LessThanOrEquals_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.Id <= 1);

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_Operator_LessThanOrEquals_Works));
        }

        [Test]
        public void Query_Same_FK_Access_Does_Not_Generate_Multiple_Joins()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.AnotherArtist.Alias == null);
            group.CreateQueryCondition<ArtistStatisticValues>(x => x.AnotherArtist.Biography != null);

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_Same_FK_Access_Does_Not_Generate_Multiple_Joins));
        }

        [Test]
        public void Query_Multiple_ConditionGroups_And_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group1 = query.CreateQueryConditionGroup();
            group1.CreateQueryCondition<ArtistStatisticValues>(x => x.AnotherArtist.Alias == null);

            var group2 = query.CreateQueryConditionGroup(QueryJunction.And);
            group2.CreateQueryCondition<ArtistStatisticValues>(x => x.AnotherArtist.Biography == null);
            group2.CreateQueryCondition<ArtistStatisticValues>(x => x.AnotherArtist.BirthDate != null);

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_Multiple_ConditionGroups_And_Works));
        }

        [Test]
        public void Query_Multiple_ConditionGroups_Or_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group1 = query.CreateQueryConditionGroup();
            group1.CreateQueryCondition<ArtistStatisticValues>(x => x.AnotherArtist.Alias == null);
            
            var group2 = query.CreateQueryConditionGroup(QueryJunction.Or);
            group2.CreateQueryCondition<ArtistStatisticValues>(x => x.AnotherArtist.Biography == null);
            group2.CreateQueryCondition<ArtistStatisticValues>(x => x.AnotherArtist.BirthDate != null);

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_Multiple_ConditionGroups_Or_Works));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void Query_Max_Results_Are_Applied_When_Not_Null(bool withMaxResults)
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.Id == 1);

            query.MaxResults = withMaxResults ? 1 : (int?)null;

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(withMaxResults 
                ? ExpectedQueryOutputs.Query_Max_Results_Are_Applied_When_Not_Null_True 
                : ExpectedQueryOutputs.Query_Max_Results_Are_Applied_When_Not_Null_False));
        }

        [Test]
        public void Query_Resolves_Sorting_Expression_Ascending()
        {
            // arrange
            var query = Query.CreateQuery<Artist>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<Artist>(x => x.AnotherArtist.Alias == "Bernd");

            query.AddSortingExpression<Artist>(x => x.AnotherArtist.Alias, SortingMode.Ascending);

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_Resolves_Sorting_Expression_Ascending));
        }

        [Test]
        public void Query_Resolves_Sorting_Expression_Descending()
        {
            // arrange
            var query = Query.CreateQuery<Artist>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<Artist>(x => x.AnotherArtist.Alias == "Bernd");

            query.AddSortingExpression<Artist>(x => x.AnotherArtist.Alias, SortingMode.Descending);

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_Resolves_Sorting_Expression_Descending));
        }

        [Test]
        public void Query_Resolves_Multiple_Sorting_Expressions()
        {
            // arrange
            var query = Query.CreateQuery<Artist>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<Artist>(x => x.AnotherArtist.Alias == "Bernd");

            query.AddSortingExpression<Artist>(x => x.AnotherArtist.Alias, SortingMode.Descending);
            query.AddSortingExpression<Artist>(x => x.Name, SortingMode.Ascending);
            query.AddSortingExpression<Artist>(x => x.AnotherArtist.Biography, SortingMode.Descending);

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_Resolves_Multiple_Sorting_Expressions));
        }

        [Test]
        public void Query_No_Sorting_Because_No_Sorting_Expressions()
        {
            // arrange
            var query = Query.CreateQuery<Artist>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<Artist>(x => x.AnotherArtist.Alias == "Bernd");

            // act
            var command = query.Command;

            // assert
            Assert.That(command, Is.EqualTo(ExpectedQueryOutputs.Query_No_Sorting));
        }

        [Test]
        public void Query_Throws_Exception_When_Sort_Expression_Can_Not_Be_Resolved()
        {
            // arrange
            var query = Query.CreateQuery<Artist>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<Artist>(x => x.Id == 1);

            query.AddSortingExpression<Artist>(x => x.AnotherArtist.Alias, SortingMode.Ascending);

            // act
            string result = null;
            Assert.Throws<QueryException>(() => result = query.Command);

            // assert
            Assert.That(result, Is.Null);
        }
    }
}

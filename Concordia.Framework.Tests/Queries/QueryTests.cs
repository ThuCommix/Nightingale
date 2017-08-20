using System.Linq;
using Concordia.Framework.Queries;
using Concordia.Framework.Queries.Tokens;
using Concordia.Framework.Tests.DataSources;
using Xunit;

namespace Concordia.Framework.Tests.Queries
{
    public class QueryTests
    {
        public QueryTests()
        {
            DependencyResolver.Clear();
            Query.RemoveQueryFilters<Artist>();
            Query.RemoveQueryFilters<ArtistStatisticValues>();
            TestHelper.SetupEntityMetadataServices();

            DependencyResolver.Register<ISqlTokenComposerService>(new SqlTokenComposerService());
        }

        [Fact]
        public void Query_Left_Join_For_Non_Mandatory_Fields_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.SecondArtist.Alias != null);

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Left_Join_For_Non_Mandatory_Fields_Works, command);
        }

        [Fact]
        public void Query_Inner_Join_For_Mandatory_Fields_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.AnotherArtist.Alias != null);

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Inner_Join_For_Mandatory_Fields_Works, command);
        }

        [Fact]
        public void Query_Null_Checks_On_Foreign_Properties_Do_Not_Create_Joins()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.SecondArtist != null);

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Null_Checks_On_Foreign_Properties_Do_Not_Create_Joins, command);
        }
        
        [Fact]
        public void Query_Operator_Equals_For_Null_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.SecondArtist == null);

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Operator_Equals_For_Null_Works, command);
        }

        [Fact]
        public void Query_Operator_Equals_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.Id == 1);

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Operator_Equals_Works, command);
        }

        [Fact]
        public void Query_Operator_NotEquals_For_Null_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.SecondArtist != null);

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Operator_NotEquals_For_Null_Works, command);
        }

        [Fact]
        public void Query_Operator_NotEquals_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.Id != 1);

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Operator_NotEquals_Works, command);
        }

        [Fact]
        public void Query_Operator_GreaterThan_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.Id > 1);

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Operator_GreaterThan_Works, command);
        }

        [Fact]
        public void Query_Operator_GreaterThanOrEquals_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.Id >= 1);

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Operator_GreaterThanOrEquals_Works, command);
        }

        [Fact]
        public void Query_Operator_LessThan_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.Id < 1);

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Operator_LessThan_Works, command);
        }

        [Fact]
        public void Query_Operator_LessThanOrEquals_Works()
        {
            // arrange
            var query = Query.CreateQuery<ArtistStatisticValues>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<ArtistStatisticValues>(x => x.Id <= 1);

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Operator_LessThanOrEquals_Works, command);
        }

        [Fact]
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
            Assert.Equal(ExpectedQueryOutputs.Query_Same_FK_Access_Does_Not_Generate_Multiple_Joins, command);
        }

        [Fact]
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
            Assert.Equal(ExpectedQueryOutputs.Query_Multiple_ConditionGroups_And_Works, command);
        }

        [Fact]
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
            Assert.Equal(ExpectedQueryOutputs.Query_Multiple_ConditionGroups_Or_Works, command);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
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
            Assert.Equal(withMaxResults
                ? ExpectedQueryOutputs.Query_Max_Results_Are_Applied_When_Not_Null_True
                : ExpectedQueryOutputs.Query_Max_Results_Are_Applied_When_Not_Null_False, command);
        }

        [Fact]
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
            Assert.Equal(ExpectedQueryOutputs.Query_Resolves_Sorting_Expression_Ascending, command);
        }

        [Fact]
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
            Assert.Equal(ExpectedQueryOutputs.Query_Resolves_Sorting_Expression_Descending, command);
        }

        [Fact]
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
            Assert.Equal(ExpectedQueryOutputs.Query_Resolves_Multiple_Sorting_Expressions, command);
        }

        [Fact]
        public void Query_No_Sorting_Because_No_Sorting_Expressions()
        {
            // arrange
            var query = Query.CreateQuery<Artist>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<Artist>(x => x.AnotherArtist.Alias == "Bernd");

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_No_Sorting, command);
        }

        [Fact]
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
            Assert.Null(result);
        }

        [Fact]
        public void Query_Apply_Global_Filters()
        {
            // arrange
            Query.SetQueryFilter<Artist>(x => x.Deleted == false);

            var query = Query.CreateQuery<Artist>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<Artist>(x => x.Id == 1);

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Global_Filter, command);
        }

        [Fact]
        public void Query_Apply_Global_Filters_Mutiple()
        {
            // arrange
            Query.SetQueryFilter<Artist>(x => x.Deleted == false);
            Query.SetQueryFilter<Artist>(x => x.Note == null);

            var query = Query.CreateQuery<Artist>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<Artist>(x => x.Id == 1);

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Global_Filter_Multiple, command);
        }

        [Fact]
        public void Query_Apply_Global_Filters_For_Correct_Entity()
        {
            // arrange
            Query.SetQueryFilter<Artist>(x => x.Deleted == false);
            Query.SetQueryFilter<ArtistStatisticValues>(x => x.AnotherArtist == null);

            var query = Query.CreateQuery<Artist>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<Artist>(x => x.Id == 1);

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Global_Filter, command);
        }

        [Fact]
        public void Query_Can_Clear_Filters()
        {
            // arrange
            Query.SetQueryFilter<Artist>(x => x.Deleted == false);
            Query.RemoveQueryFilters<Artist>();

            var query = Query.CreateQuery<Artist>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition<Artist>(x => x.Id == 1);

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_No_Filter, command);
        }

        [Fact]
        public void Query_Supports_Complex_Expressions()
        {
            // arrange
            var query = Query.CreateQuery<Artist>();
            var group = query.CreateQueryConditionGroup();
            group.CreateQueryCondition<Artist>(x => x.Name == "Artist" && x.AnotherArtist.Note == "Test" || x.Alias == "Fresh Artist" && x.AnotherArtist.AnotherArtist.Note != "Test");

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Supports_Complex_Expressions, command);
        }

        [Fact]
        public void Query_Supports_Complex_Expressions2()
        {
            // arrange
            var query = Query.CreateQuery<Artist>();
            var group = query.CreateQueryConditionGroup();
            group.CreateQueryCondition<Artist>(x => x.Name.StartsWith("Test") && x.AnotherArtist == null || x.AnotherArtist.AnotherArtist.Name == "Peter");

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Supports_Complex_Expressions2, command);
        }

        [Fact]
        public void Query_Supports_String_StartsWith()
        {
            // arrange
            var query = Query.CreateQuery<Artist>();
            var group = query.CreateQueryConditionGroup();
            group.CreateQueryCondition<Artist>(x => x.Name.StartsWith("Artist") && x.Alias == "Alias");

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Supports_String_StartsWith, command);
            Assert.EndsWith("%", query.Parameters.First().Value.ToString());
        }

        [Fact]
        public void Query_Supports_String_EndsWith()
        {
            // arrange
            var query = Query.CreateQuery<Artist>();
            var group = query.CreateQueryConditionGroup();
            group.CreateQueryCondition<Artist>(x => x.Name.EndsWith("Artist") && x.Alias == "Alias");

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Supports_String_EndsWith, command);
            Assert.StartsWith("%", query.Parameters.First().Value.ToString());
        }

        [Fact]
        public void Query_Supports_String_Contains()
        {
            // arrange
            var query = Query.CreateQuery<Artist>();
            var group = query.CreateQueryConditionGroup();
            group.CreateQueryCondition<Artist>(x => x.Name.Contains("Artist") && x.Alias == "Alias");

            // act
            var command = query.Command;

            // assert
            Assert.Equal(ExpectedQueryOutputs.Query_Supports_String_Contains, command);
            Assert.StartsWith("%", query.Parameters.First().Value.ToString());
            Assert.EndsWith("%", query.Parameters.First().Value.ToString());
        }
    }
}

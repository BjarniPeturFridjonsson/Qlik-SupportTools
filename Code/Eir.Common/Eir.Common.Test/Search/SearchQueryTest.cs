using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Eir.Common.Search;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Eir.Common.Test.Search
{
    [TestFixture]
    public class SearchQueryTest : TestBase
    {
        private static void AssertResultSet(IEnumerable<TestModel> result, params int[] expectedNumbers)
        {
            int[] actualNumbers = result.Select(x => x.Number).ToArray();

            Console.WriteLine("Expected numbers: " + string.Join(", ", expectedNumbers));
            Console.WriteLine("Actual numbers:   " + string.Join(", ", actualNumbers));

            CollectionAssert.AreEqual(expectedNumbers, actualNumbers);
        }

        [Test]
        public void SerializeDeserialize()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>(LogicalOp.Or)
                .AddComparer(x => x.Name, CompareOp.Equals, "Name")
                .AddComparer(x => x.Name, CompareOp.Equals, "name")
                .AddComparerGroup(LogicalOp.None)
                .AddComparer(x => x.Number, CompareOp.GreaterThan, 2);

            Console.WriteLine(searchQueryBuilder.Builder);

            string json = JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery());

            SearchQuery searchQuery = JsonConvert.DeserializeObject<SearchQuery>(json);

            Console.WriteLine(searchQuery);

            // Test during development... I.e. checking the searchQuery object manually...
            // Not actually testing anything here...  :-/
        }

        [Test]
        public void ToStringTest()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>(LogicalOp.Or)
                .AddComparer(x => x.Name, CompareOp.Equals, "Name")
                .AddComparer(x => x.Name, CompareOp.Equals, "name")
                .AddComparerGroup(LogicalOp.And)
                .AddComparer(x => x.Number, CompareOp.GreaterThan, 2)
                .AddComparer(x => x.Number, CompareOp.LessThan, 4)
                .Parent;

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            var asString = searchQueryBuilder.ToString();

            Assert.AreEqual("((Name == \"Name\") || (Name == \"name\") || ((Number > \"2\") && (Number < \"4\")))", asString);
        }

        [Test]
        public void SimpleSearch()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>(LogicalOp.Or)
                .AddComparer(x => x.Name, CompareOp.Equals, "Name")
                .AddComparer(x => x.Name, CompareOp.Equals, "name")
                .AddComparerGroup(LogicalOp.None)
                .AddComparer(x => x.Number, CompareOp.GreaterThan, 2);

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

            IEnumerable<TestModel> testData = GetTestData();
            Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
            var result = testData.Where(expression.Compile());

            AssertResultSet(result, 3, 4, 5, 6, 7);
        }

        [Test]
        public void EqualsIgnoreCase()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>()
                .AddComparer(x => x.Name, CompareOp.Equals, "Name");

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

            IEnumerable<TestModel> testData = GetTestData();
            Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
            var result = testData.Where(expression.Compile());

            AssertResultSet(result, 3, 4);
        }

        [Test]
        public void EqualsCaseSensitive_no_match()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>()
                .AddComparer(x => x.Name, CompareOp.Equals, "no match");

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

            IEnumerable<TestModel> testData = GetTestData();
            Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
            var result = testData.Where(expression.Compile());

            AssertResultSet(result);
        }

        //[Test]
        //public void EqualsCaseSensitive()
        //{
        //    var searchQueryBuilder = new SearchQueryBuilder<TestModel>()
        //        .AddComparer(x => x.Name, CompareOp.EqualsCaseSensitive, "Name");

        //    Console.WriteLine(searchQueryBuilder.Builder);
        //    Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

        //    SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

        //    IEnumerable<TestModel> testData = GetTestData();
        //    Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
        //    var result = testData.Where(expression.Compile());

        //    AssertResultSet(result, 3);
        //}

        [Test]
        public void String_StartsWith()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>()
                .AddComparer(x => x.Name, CompareOp.StartsWith, "N");

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

            IEnumerable<TestModel> testData = GetTestData();
            Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
            var result = testData.Where(expression.Compile());

            AssertResultSet(result, 3, 4, 5);
        }

        [Test]
        public void String_EndssWith()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>()
                .AddComparer(x => x.Name, CompareOp.EndsWith, " Name");

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

            IEnumerable<TestModel> testData = GetTestData();
            Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
            var result = testData.Where(expression.Compile());

            AssertResultSet(result, 1, 2, 5);
        }


        [Test]
        public void String_Contains()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>()
                .AddComparer(x => x.Name, CompareOp.Contains, "o");

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

            IEnumerable<TestModel> testData = GetTestData();
            Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
            var result = testData.Where(expression.Compile());

            Assert.AreEqual(3, result.Count());
        }

        [Test]
        public void DateTime_interval()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>(LogicalOp.And)
                .AddComparer(x => x.Timestamp, CompareOp.GreaterThan, CreateDateTime(2000, 1, 2))
                .AddComparer(x => x.Timestamp, CompareOp.LessThan, CreateDateTime(2000, 1, 5));

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

            IEnumerable<TestModel> testData = GetTestData();
            Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
            var result = testData.Where(expression.Compile());

            AssertResultSet(result, 5, 6);
        }

        [Test]
        public void Nullable_DateTime_interval()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>(LogicalOp.And)
                .AddComparer(x => x.NullableTimestamp, CompareOp.GreaterThan, CreateDateTime(2001, 1, 1));

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

            IEnumerable<TestModel> testData = GetTestData();
            Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
            var result = testData.Where(expression.Compile());

            AssertResultSet(result, 1, 4);
        }

        [Test]
        public void Nullable_DateTime_is_null()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>(LogicalOp.And)
                .AddComparer(x => x.NullableTimestamp, CompareOp.Equals, null);

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

            IEnumerable<TestModel> testData = GetTestData();
            Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
            var result = testData.Where(expression.Compile());

            AssertResultSet(result, 2, 5, 6, 7);
        }

        [Test]
        public void Nullable_DateTime_is_not_null()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>(LogicalOp.And)
                .AddComparer(x => x.NullableTimestamp, CompareOp.NotEquals, null);

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

            IEnumerable<TestModel> testData = GetTestData();
            Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
            var result = testData.Where(expression.Compile());

            AssertResultSet(result, 1, 3, 4);
        }

        [Test]
        public void Nullable_Number_is_null()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>(LogicalOp.And)
                .AddComparer(x => x.NullableNumber, CompareOp.Equals, null);

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

            IEnumerable<TestModel> testData = GetTestData();
            Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
            var result = testData.Where(expression.Compile());

            AssertResultSet(result, 2, 4, 6, 7);
        }

        [Test]
        public void Nullable_Number_is_not_null()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>(LogicalOp.And)
                .AddComparer(x => x.NullableNumber, CompareOp.NotEquals, null);

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

            IEnumerable<TestModel> testData = GetTestData();
            Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
            var result = testData.Where(expression.Compile());

            AssertResultSet(result, 1, 3, 5);
        }


        [Test]
        public void String_Match_is_case_insensitive()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>()
                .AddComparer(x => x.Name, CompareOp.Match, "Name");

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

            IEnumerable<TestModel> testData = GetTestData();
            Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
            var result = testData.Where(expression.Compile());

            AssertResultSet(result, 3, 4);
        }

        [Test]
        public void String_Match_starts_with_wildcard()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>()
                .AddComparer(x => x.Name, CompareOp.Match, "* NAME");

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

            IEnumerable<TestModel> testData = GetTestData();
            Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
            var result = testData.Where(expression.Compile());

            AssertResultSet(result, 1, 2, 5);
        }

        [Test]
        public void String_Match_ends_with_wildcard()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>()
                .AddComparer(x => x.Name, CompareOp.Match, "n*");

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

            IEnumerable<TestModel> testData = GetTestData();
            Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
            var result = testData.Where(expression.Compile());

            AssertResultSet(result, 3, 4, 5);
        }

        [Test]
        public void String_Match_starts_and_ends_with_wildcard()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>()
                .AddComparer(x => x.Name, CompareOp.Match, "*o*");

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

            IEnumerable<TestModel> testData = GetTestData();
            Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
            var result = testData.Where(expression.Compile());

            AssertResultSet(result, 2, 5, 6);
        }

        [Test]
        public void String_Match_nothing()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>()
                .AddComparer(x => x.Name, CompareOp.Match, "no match at all");

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            SearchQuery searchQuery = searchQueryBuilder.ToSearchQuery();

            IEnumerable<TestModel> testData = GetTestData();
            Expression<Func<TestModel, bool>> expression = SearchExpressionBuilder.CreateSearchExpression<TestModel>(searchQuery, SearchIntent.InMemory);
            var result = testData.Where(expression.Compile());

            AssertResultSet(result);
        }


        [Test]
        public void Filter_empty()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>();

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            IEnumerable<TestModel> result = GetTestData().Filter(searchQueryBuilder.ToSearchQuery(), SearchIntent.InMemory);

            AssertResultSet(result, 1, 2, 3, 4, 5, 6, 7);
        }

        [Test]
        public void Filter_OrderBy_Timestamp()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>()
                .AddOrderBy(x => x.Timestamp);

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            IEnumerable<TestModel> result = GetTestData().Filter(searchQueryBuilder.ToSearchQuery(), SearchIntent.InMemory);

            AssertResultSet(result, 3, 2, 6, 5, 1, 4, 7);
        }

        [Test]
        public void Filter_OrderBy_Timestamp_desc()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>()
                .AddOrderBy(x => x.Timestamp, SortOrder.Descending);

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            IEnumerable<TestModel> result = GetTestData().Filter(searchQueryBuilder.ToSearchQuery(), SearchIntent.InMemory);

            AssertResultSet(result, 7, 4, 1, 5, 6, 2, 3);
        }

        [Test]
        public void Filter_OrderBy_Name_then_Number()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>()
                .AddOrderBy(x => x.Name)
                .AddOrderBy(x => x.Number);

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            IEnumerable<TestModel> result = GetTestData().Filter(searchQueryBuilder.ToSearchQuery(), SearchIntent.InMemory);

            AssertResultSet(result, 7, 1, 2, 3, 4, 5, 6);
        }

        [Test]
        public void Filter_OrderBy_Name_Desc_then_Number()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>()
                .AddOrderBy(x => x.Name, SortOrder.Descending)
                .AddOrderBy(x => x.Number);

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            IEnumerable<TestModel> result = GetTestData().Filter(searchQueryBuilder.ToSearchQuery(), SearchIntent.InMemory);

            AssertResultSet(result, 6, 5, 3, 4, 2, 1, 7);
        }

        [Test]
        public void Filter_OrderBy_Name_then_Number_Desc()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>()
                .AddOrderBy(x => x.Name)
                .AddOrderBy(x => x.Number, SortOrder.Descending);

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            IEnumerable<TestModel> result = GetTestData().Filter(searchQueryBuilder.ToSearchQuery(), SearchIntent.InMemory);

            AssertResultSet(result, 7, 1, 2, 4, 3, 5, 6);
        }

        [Test]
        public void Filter_OrderBy_Name_Desc_then_Number_Desc()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>()
                .AddOrderBy(x => x.Name, SortOrder.Descending)
                .AddOrderBy(x => x.Number, SortOrder.Descending);

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            IEnumerable<TestModel> result = GetTestData().Filter(searchQueryBuilder.ToSearchQuery(), SearchIntent.InMemory);

            AssertResultSet(result, 6, 5, 4, 3, 2, 1, 7);
        }

        [Test]
        public void Filter_skip_2()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>().Skip(2);

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            IEnumerable<TestModel> result = GetTestData().Filter(searchQueryBuilder.ToSearchQuery(), SearchIntent.InMemory);

            AssertResultSet(result, 3, 4, 5, 6, 7);
        }

        [Test]
        public void Filter_take_3()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>().Take(3);

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            IEnumerable<TestModel> result = GetTestData().Filter(searchQueryBuilder.ToSearchQuery(), SearchIntent.InMemory);

            AssertResultSet(result, 1, 2, 3);
        }

        [Test]
        public void Filter_skip_2_take_3()
        {
            var searchQueryBuilder = new SearchQueryBuilder<TestModel>().Skip(2).Take(3);

            Console.WriteLine(searchQueryBuilder.Builder);
            Console.WriteLine(JsonConvert.SerializeObject(searchQueryBuilder.ToSearchQuery()));

            IEnumerable<TestModel> result = GetTestData().Filter(searchQueryBuilder.ToSearchQuery(), SearchIntent.InMemory);

            AssertResultSet(result, 3, 4, 5);
        }


        private static IEnumerable<TestModel> GetTestData()
        {
            yield return new TestModel
            {
                Name = "A name",
                Number = 1,
                Timestamp = CreateDateTime(2000, 1, 5),
                NullableTimestamp = CreateDateTime(2001, 1, 3),
                NullableNumber = 3
            };

            yield return new TestModel
            {
                Name = "Another Name",
                Number = 2,
                Timestamp = CreateDateTime(2000, 1, 2),
                NullableTimestamp = null,
                NullableNumber = null
            };

            yield return new TestModel
            {
                Name = "Name",
                Number = 3,
                Timestamp = CreateDateTime(2000, 1, 1),
                NullableTimestamp = CreateDateTime(2001, 1, 1),
                NullableNumber = 2
            };

            yield return new TestModel
            {
                Name = "name",
                Number = 4,
                Timestamp = CreateDateTime(2000, 1, 6),
                NullableTimestamp = CreateDateTime(2001, 1, 2),
                NullableNumber = null
            };

            yield return new TestModel
            {
                Name = "Not That Name",
                Number = 5,
                Timestamp = CreateDateTime(2000, 1, 4),
                NullableTimestamp = null,
                NullableNumber = 1
            };

            yield return new TestModel
            {
                Name = "SOME DUDE",
                Number = 6,
                Timestamp = CreateDateTime(2000, 1, 3),
                NullableTimestamp = null,
                NullableNumber = null
            };

            yield return new TestModel
            {
                Name = null,
                Number = 7,
                Timestamp = CreateDateTime(2000, 1, 7),
                NullableTimestamp = null,
                NullableNumber = null
            };
        }

        private static DateTime CreateDateTime(int year, int month, int day)
        {
            return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        }

        public class TestModel
        {
            public string Name { get; set; }

            public int Number { get; set; }

            public DateTime Timestamp { get; set; }

            public DateTime? NullableTimestamp { get; set; }

            public int? NullableNumber { get; set; }

            public override string ToString()
            {
                return $"#{Number}, \"{Name}\", {Timestamp.ToString("yyyy'-'MM'-'dd")}, " +
                       $"{NullableTimestamp?.ToString("yyyy'-'MM'-'dd") ?? "null"}, " +
                       $"{NullableNumber?.ToString() ?? "null"}";
            }
        }
    }
}
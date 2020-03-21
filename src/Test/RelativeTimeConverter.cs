using System;
using System.Collections.Generic;
using Events;
using Shouldly;
using Xunit;

namespace Test
{
    public class RelativeTimeConverterTest
    {
        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[] { "Connected 1 day ago", DateTimeOffset.Now.AddDays(-1).Date },
                new object[] { "Connected 2 days ago", DateTimeOffset.Now.AddDays(-2).Date },
                new object[] { "Connected 1 week ago", DateTimeOffset.Now.AddDays(-7).Date },
                new object[] { "Connected 2 weeks ago", DateTimeOffset.Now.AddDays(-14).Date},
                new object[] { "Connected 1 month ago", DateTimeOffset.Now.AddMonths(-1).Date},
                new object[] { "Connected 2 months ago", DateTimeOffset.Now.AddMonths(-2).Date },
                new object[] { "Connected 1 year ago", DateTimeOffset.Now.AddYears(-1).Date },
                new object[] { "Connected 2 years ago", DateTimeOffset.Now.AddYears(-2).Date },
            };

        [Theory]
        [MemberData(nameof(Data))]
        public void ConvertsTime(string value, DateTimeOffset expected)
        {
            var result = new RelativeTimeConverter().Convert(value);

            result.ShouldBe(expected);
        }
    }
}

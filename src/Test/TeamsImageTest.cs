using System.Linq;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;
using Events;

namespace Test
{
    public class TeamsImageTest
    {
        [Fact]
        public void ShouldParseOneCorrectly()
        {
            var message = "<div><img src=\"https://something=/$value\" style=\"width:100px; height:200px\"></div>";
            var result = new ImageProcessor().ExtractImages(message);
            result.ShouldNotBeEmpty();
            result.Length.ShouldBe(1);
        }

        [Fact]
        public void ShouldParseTwoCorrectly()
        {
            var message = "<div><img src=\"https://something=/$value\" style=\"width:100px; height:200px\"></div><div><img src=\"https://something=/$value\" style=\"width:100px; height:200px\"></div>";
            var result = new ImageProcessor().ExtractImages(message);
            result.Length.ShouldBe(2);
        }

        [Fact]
        public void ShoudlParseNothing()
        {
            var message = "nonsense";
            var result = new ImageProcessor().ExtractImages(message);
            result.ShouldBeEmpty();
        }
    }
}
using System;
using Ipfs.Utilities;
using Xunit;

namespace Ipfs.Test
{
    public class UriHelperTests
    {
        [Fact]
        public void ShouldAppendPathsCorrectly()
        {
            var baseUri = new Uri("http://127.0.0.1:5001");

            string firstPath = "/api/v0/";
            const string FIRST_EXPECTED_PATH = "http://127.0.0.1:5001/api/v0";
            var firstUri = UriHelper.AppendPath(baseUri, firstPath);

            Assert.True(Equals(firstUri, FIRST_EXPECTED_PATH));

            const string SECOND_PATH = "/methodName";
            string secondExpectedPath = "http://127.0.0.1:5001/api/v0/methodName";

            var secondUri = UriHelper.AppendPath(firstUri, SECOND_PATH);

            Assert.True(Equals(secondUri, secondExpectedPath));
        }

        //[Fact]
        //public void ShouldAppendQueryCorrectly()
        //{
        //    Uri baseUri = new Uri("http://127.0.0.1:5001/api/v0/methodName");
        //    var query = new Dictionary<string, string>
        //    {
        //        { "arg", "myArg" }
        //    };

        //    string expectedUri = "http://127.0.0.1:5001/api/v0/methodName?arg=myArg";
        //    Uri actualUri = UriHelper.AppendQuery(baseUri, query);
        //    Assert.True(Equals(expectedUri, actualUri));
        //}
    }
}

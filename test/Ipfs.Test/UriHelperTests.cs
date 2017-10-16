using Ipfs.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Ipfs.Test
{
    [TestClass]
    public class UriHelperTests
    {
        [TestMethod]
        public void ShouldAppendPathsCorrectly()
        {
            Uri baseUri = new Uri("http://127.0.0.1:5001");

            string firstPath = "/api/v0/";
            string firstExpectedPath = "http://127.0.0.1:5001/api/v0";
            Uri firstUri = UriHelper.AppendPath(baseUri, firstPath);

            Assert.IsTrue(Equals(firstUri, firstExpectedPath));

            string secondPath = "/methodName";
            string secondExpectedPath = "http://127.0.0.1:5001/api/v0/methodName";

            Uri secondUri = UriHelper.AppendPath(firstUri, secondPath);

            Assert.IsTrue(Equals(secondUri, secondExpectedPath));
        }

        //[TestMethod]
        //public void ShouldAppendQueryCorrectly()
        //{
        //    Uri baseUri = new Uri("http://127.0.0.1:5001/api/v0/methodName");
        //    var query = new Dictionary<string, string>
        //    {
        //        { "arg", "myArg" }
        //    };

        //    string expectedUri = "http://127.0.0.1:5001/api/v0/methodName?arg=myArg";
        //    Uri actualUri = UriHelper.AppendQuery(baseUri, query);
        //    Assert.IsTrue(Equals(expectedUri, actualUri));
        //}
    }
}

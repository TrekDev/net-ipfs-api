﻿using Ipfs.Test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Ipfs.Test
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void ClientShouldThrowIfMethodCalledAfterBeingDisposed()
        {
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK);
            mockResponse.Content = new StringContent(String.Empty);

            var mockHttpMessageHandler = new MockHttpMessageHandler(mockResponse);
            string mockAddress = "http://127.0.0.1:5001";
            var client = new IpfsClient(new Uri(mockAddress), new HttpClient(mockHttpMessageHandler));

            client.Dispose();

            try
            {
                client.Commands().Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
        }

        [TestMethod]
        public async Task ClientShouldBeAbleToDownloadLargeFiles()
        {
           /* This test is a bit long because it relies of having 
            * a large file available through IPFS and the best way to
            * ensure that is to simply create that file in the first place */
            
            var sourceFile = Path.GetTempFileName();
            var targetFile = Path.GetTempFileName();

            try
            {
                using (var client = new IpfsClient())
                {
                    string sourceHash;                    
                    await CreateDummyFileAsync(sourceFile);

                    using (var sourceStream = File.OpenRead(sourceFile))
                    using (var ipfsSourceStream = new IpfsStream("source", sourceStream))
                    {
                        var hash = await client.Add(ipfsSourceStream);
                        sourceHash = hash.ToString();
                    }

                    using (var stream = await client.Cat(sourceHash))
                    using (var outputFilename = File.OpenWrite(targetFile))
                    {
                        await stream.CopyToAsync(outputFilename);
                    }

                    await client.Pin.Rm(sourceHash);

                    Assert.IsTrue(FileHashesAreEqual(sourceFile, targetFile));
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (File.Exists(sourceFile))
                {
                    File.Delete(sourceFile);
                }

                if (File.Exists(targetFile))
                {
                    File.Delete(targetFile);
                }
            }
        }

        private static async Task CreateDummyFileAsync(string filename, int sizeMb = 500)
        {
            using (var stream = File.OpenWrite(filename))
            {
                var buffer = new byte[1024 * 1024];

                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = 0xFF;
                }

                for (int i = 0; i < sizeMb; i++)
                {
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
        }

        private static bool FileHashesAreEqual(string leftFile, string rightFile)
        {
            using (var sha = new SHA256CryptoServiceProvider())
            {
                using (var leftStream = File.OpenRead(leftFile))
                using (var rightStream = File.OpenRead(rightFile))
                {
                    var leftHash = sha.ComputeHash(leftStream);
                    var rightHash = sha.ComputeHash(rightStream);
                    return leftHash.SequenceEqual(rightHash);
                }
            }
        }
    }
}

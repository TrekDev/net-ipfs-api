using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ipfs.Json;

namespace Ipfs.Commands
{
    public class IpfsBlock : IpfsCommand
    {
        internal IpfsBlock(Uri commandUri, HttpClient httpClient, IJsonSerializer jsonSerializer) : base(commandUri, httpClient, jsonSerializer)
        {
        }

        /// <summary>
        /// ipfs block get <key> - Get a raw IPFS block
        /// 'ipfs block get' is a plumbing command for retreiving raw ipfs blocks.
        /// </summary>
        /// <param name="key">The base58 multihash of an existing block to get</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<byte[]> Get(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            var content = await ExecuteGetAsync("get", key, cancellationToken);

            return await content.ReadAsByteArrayAsync();
        }

        /// <summary>
        /// ipfs block put <data> - Stores input as an IPFS block
        /// ipfs block put is a plumbing command for storing raw ipfs blocks.
        /// It reads from stdin, and <key> is a base58 encoded multihash.
        /// </summary>
        /// <param name="data">The data to be stored as an IPFS block</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<HttpContent> Put(byte[] data, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await ExecuteGetAsync("put", data.ToString(), cancellationToken);
        }

        /// <summary>
        /// Print information of a raw IPFS block
        /// </summary>
        /// <param name="key">The base58 multihash of an existing block to get</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<HttpContent> Stat(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await ExecuteGetAsync("get", key, cancellationToken);
        }
    }
}

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ipfs.Json;

namespace Ipfs.Commands
{
    public class IpfsTour : IpfsCommand
    {
        internal IpfsTour(Uri commandUri, HttpClient httpClient, IJsonSerializer jsonSerializer) : base(commandUri, httpClient, jsonSerializer)
        {
        }

        /// <summary>
        /// Show a list of IPFS Tour topics
        /// </summary>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns>Stream to tour topcs</returns>
        public async Task<Stream> List(CancellationToken cancellationToken = default(CancellationToken))
        {
            var content = await ExecuteGetAsync("list", cancellationToken);

            return await content.ReadAsStreamAsync();
        }

        /// <summary>
        /// Show the next IPFS Tour topic
        /// </summary>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns>Next tour topic</returns>
        public async Task<Stream> Next(CancellationToken cancellationToken = default(CancellationToken))
        {
            var content = await ExecuteGetAsync("next", cancellationToken);

            return await content.ReadAsStreamAsync();
        }

        /// <summary>
        /// Restart the IPFS Tour
        /// </summary>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        public async Task Restart(CancellationToken cancellationToken = default(CancellationToken))
        {
            await ExecuteGetAsync("restart", cancellationToken);
        }
    }
}

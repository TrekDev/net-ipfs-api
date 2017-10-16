using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ipfs.Json;

namespace Ipfs.Commands
{
    public class IpfsRefs : IpfsCommand
    {
        internal IpfsRefs(Uri commandUri, HttpClient httpClient, IJsonSerializer jsonSerializer) : base(commandUri, httpClient, jsonSerializer)
        {
        }

        /// <summary>
        /// Lists all local references
        /// Displays the hashes of all local objects.
        /// </summary>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns>Returns an enumerable of multihashes, or an empty enumerable if nothing is found.</returns>
        public async Task<IEnumerable<MultiHash>> Local(CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpContent content = await ExecuteGetAsync("local", cancellationToken);

            //DOESN'T return json!
            //Returns a list of multihashes separated by "\n"
            var stringContent = await content.ReadAsStringAsync();

            if(String.IsNullOrEmpty(stringContent))
            {
                return Enumerable.Empty<MultiHash>();
            }

            return stringContent
                .Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => new MultiHash(x));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ipfs.Json;

namespace Ipfs.Commands
{
    public class IpfsRepo : IpfsCommand
    {
        internal IpfsRepo(Uri commandUri, HttpClient httpClient, IJsonSerializer jsonSerializer) : base(commandUri, httpClient, jsonSerializer)
        {
        }

        /// <summary>
        /// Perform a garbage collection sweep on the repo
        /// 
        /// 'ipfs repo gc' is a plumbing command that will sweep the local
        /// set of stored objects and remove ones that are not pinned in
        /// order to reclaim hard disk space.
        /// </summary>
        /// <param name="quiet">Write minimal output</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns>An enumerable of multihashes that were cleared. The enumerable will be empty if no entries were removed</returns>
        public async Task<IEnumerable<MultiHash>> GC(bool quiet = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var flags = new Dictionary<string, string>();

            if(quiet)
            {
                flags.Add("quiet", "true");
            }

            var content = await ExecuteGetAsync("gc", flags, cancellationToken);

            string json = await content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(json))
            {
                return Enumerable.Empty<MultiHash>();
            }

            var keys = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            return keys.Values.Select(x => new MultiHash(x));
        }
    }
}

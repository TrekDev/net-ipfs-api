﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ipfs.Json;

namespace Ipfs.Commands
{
    public class IpfsDht : IpfsCommand
    {
        internal IpfsDht(Uri commandUri, HttpClient httpClient, IJsonSerializer jsonSerializer) : base(commandUri, httpClient, jsonSerializer)
        {
        }

        /// <summary>
        /// Run a 'FindPeer' query through the DHT
        /// </summary>
        /// <param name="peerId">The peer to search for</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<HttpContent> FindPeer(string peerId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await ExecuteGetAsync("findpeer", peerId, cancellationToken);
        }

        /// <summary>
        /// Run a 'FindProviders' query through the DHT
        /// FindProviders will return a list of peers who are able to provide the value requested.
        /// </summary>
        /// <param name="key">The key to find providers for</param>
        /// <param name="verbose">Write extra information</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<HttpContent> FindProvs(string key, bool verbose = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var flags = new Dictionary<string, string>();

            if(verbose)
            {
                flags.Add("verbose", "true");
            }

            return await ExecuteGetAsync("findprovs", key, flags, cancellationToken);
        }

        /// <summary>
        /// Run a 'findClosestPeers' query through the DHT
        /// </summary>
        /// <param name="peerId">The peerID to run the query against</param>
        /// <param name="verbose">Write extra information</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<HttpContent> Query(string peerId, bool verbose = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var flags = new Dictionary<string, string>();

            if (verbose)
            {
                flags.Add("verbose", "true");
            }

            return await ExecuteGetAsync("findprovs", peerId, flags, cancellationToken);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ipfs.Json;

namespace Ipfs.Commands
{
    public class IpfsBitSwap : IpfsCommand
    {
        internal IpfsBitSwap(Uri commandUri, HttpClient httpClient, IJsonSerializer jsonSerializer) : base(commandUri, httpClient, jsonSerializer)
        {
        }

        /// <summary>
        /// Show some diagnostic information on the bitswap agent
        /// </summary>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns>IpfsBitSwapStat object</returns>
        public async Task<IpfsBitSwapStat> Stat(CancellationToken cancellationToken = default(CancellationToken))
        {
            var content = await ExecuteGetAsync("stat", cancellationToken);

            string json = await content.ReadAsStringAsync();

            var stat = JsonSerializer.Deserialize<Json.IpfsBitSwapStat>(json);

            return new IpfsBitSwapStat
            {
                ProvideBufLen = stat.ProvideBufLen,
                Wantlist = stat.Wantlist,
                Peers = stat.Peers?.Select(x => new MultiHash(x)).ToList(),
                BlocksReceived = stat.BlocksReceived,
                DupBlksReceived = stat.DupBlksReceived,
                DupDataReceived = stat.DupDataReceived
            };
        }

        /// <summary>
        /// Remove a given block from your wantlist
        /// </summary>
        /// <param name="key">key to remove from your wantlist</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<HttpContent> Unwant(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await ExecuteGetAsync("unwant", key, cancellationToken);
        }

        /// <summary>
        /// Show blocks currently on the wantlist
        /// 
        /// Print out all blocks currently on the bitswap wantlist for the local peer
        /// </summary>
        /// <param name="peer">specify which peer to show wantlist for (default self)</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<HttpContent> Wantlist(string peer = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var flags = new Dictionary<string, string>();

            if(!string.IsNullOrEmpty(peer))
            {
                flags.Add("peer", peer);
            }

            return await ExecuteGetAsync("wantlist", flags, cancellationToken);
        }
    }
}

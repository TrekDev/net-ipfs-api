using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Ipfs.Json;
using System.Threading;

namespace Ipfs.Commands
{
    public class IpfsDiag : IpfsCommand
    {
        internal IpfsDiag(Uri commandUri, HttpClient httpClient, IJsonSerializer jsonSerializer) : base(commandUri, httpClient, jsonSerializer)
        {
        }

        /// <summary>
        /// Generates a network diagnostics report
        /// 
        /// Sends out a message to each node in the network recursively
        /// requesting a listing of data about them including number of
        /// connected peers and latencies between them.
        /// 
        /// The given timeout will be decremented 2s at every network hop,
        /// ensuring peers try to return their diagnostics before the initiator's
        /// timeout.If the timeout is too small, some peers may not be reached.
        /// 30s and 60s are reasonable timeout values, though network vary.
        /// The default timeout is 20 seconds.
        /// 
        /// The 'vis' option may be used to change the output format.
        /// four formats are supported:
        /// * plain text - easy to read
        /// * d3 - json ready to be fed into d3view
        /// * dot - graphviz format
        /// 
        /// The d3 format will output a json object ready to be consumed by
        /// the chord network viewer, available at the following hash:
        /// 
        /// /ipfs/QmbesKpGyQGd5jtJFUGEB1ByPjNFpukhnKZDnkfxUiKn38
        /// 
        /// To view your diag output, 'ipfs add' the d3 vis output, and
        /// open the following link:
        /// 
        /// https//ipfs.io/ipfs/QmbesKpGyQGd5jtJFUGEB1ByPjNFpukhnKZDnkfxUiKn38/chord#<your hash>
        /// 
        /// The dot format can be fed into graphviz and other programs
        /// that consume the dot format to generate graphs of the network.
        /// </summary>
        /// <param name="timeout">diagnostic timeout duration</param>
        /// <param name="vis">output vis. one of: d3, dot. Default d3</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns>UTF-8 encoded byte array of text, d3 or dot format of network graph, depending on vis parameter</returns>
        public async Task<byte[]> Net(string timeout = null, IpfsVis? vis = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var flags = new Dictionary<string, string>();

            if(timeout != null)
            {
                flags.Add("timeout", timeout);
            }

            if(vis!= null)
            {
                string visValue = null;

                switch (vis.Value)
                {
                    case IpfsVis.D3:
                        visValue = "d3";
                        break;
                    case IpfsVis.Dot:
                        visValue = "dot";
                        break;
                    default:
                        break;
                }

                flags.Add("vis", visValue);
            }

            HttpContent content = await ExecuteGetAsync("net", flags, cancellationToken);

            return await content.ReadAsByteArrayAsync();
        }
    }
}

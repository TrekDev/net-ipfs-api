using Ipfs.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ipfs.Commands
{
    public class IpfsSwarm : IpfsCommand
    {
        internal IpfsSwarm(Uri commandUri, HttpClient httpClient, IJsonSerializer jsonSerializer) : base(commandUri, httpClient, jsonSerializer)
        {
        }

        /// <summary>
        /// List known addresses. Useful to debug.
        /// 
        /// ipfs swarm addrs lists all addresses this node is aware of.
        /// </summary>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns>all addresses this node is aware of</returns>
        public async Task<IEnumerable<IpfsPeer>> Addrs(CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpContent content = await ExecuteGetAsync("addrs", cancellationToken);

            string json = await content.ReadAsStringAsync();

            if(String.IsNullOrEmpty(json))
            {
                return Enumerable.Empty<IpfsPeer>();
            }

            var jsonDict = _jsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<string>>>>(json);

            return jsonDict["Addrs"].Select(x => new IpfsPeer(new MultiHash(x.Key), x.Value.Select(y => new MultiAddress(y))));
        }

        /// <summary>
        /// Open connection to a given address
        /// 
        /// 'ipfs swarm connect' opens a connection to a peer address. The address format
        /// is an ipfs multiaddr:
        /// 
        /// ipfs swarm connect /ip4/104.131.131.82/tcp/4001/ipfs/QmaCpDMGvV2BGHeYERUEnRQAwe3N8SzbUtfsmvsqQLuvuJ
        /// </summary>
        /// <param name="address">address of peer to connect to</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<HttpContent> Connect(string address, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await ExecuteGetAsync("connect", address, cancellationToken);
        }

        /// <summary>
        /// Close connection to a given address
        /// 
        /// 'ipfs swarm disconnect' closes a connection to a peer address. The address format
        /// is an ipfs multiaddr:
        /// 
        /// ipfs swarm disconnect /ip4/104.131.131.82/tcp/4001/ipfs/QmaCpDMGvV2BGHeYERUEnRQAwe3N8SzbUtfsmvsqQLuvuJ
        /// </summary>
        /// <param name="multiAddress">address of peer to disconnect from</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns>'disconnect <address> successs' on success</returns>
        public async Task<IpfsPeerConnectionStatus> Disconnect(MultiAddress multiAddress, CancellationToken cancellationToken = default(CancellationToken))
        {
            var ret = await Disconnect(ToEnumerable(multiAddress), cancellationToken);
            return ret.First();
        }

        /// <summary>
        /// Close connection to a given address
        /// 
        /// 'ipfs swarm disconnect' closes a connection to a peer address. The address format
        /// is an ipfs multiaddr:
        /// 
        /// ipfs swarm disconnect /ip4/104.131.131.82/tcp/4001/ipfs/QmaCpDMGvV2BGHeYERUEnRQAwe3N8SzbUtfsmvsqQLuvuJ
        /// </summary>
        /// <param name="multiAddress">address of peer to disconnect from</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns>'disconnect <address> successs' on success</returns>
        public async Task<IpfsPeerConnectionStatus> Disconnect(string multiAddress, CancellationToken cancellationToken = default(CancellationToken))
        {
            var ret = await Disconnect(ToEnumerable(multiAddress), cancellationToken);
            return ret.First();
        }

        /// <summary>
        /// Close connection to a given address
        /// 
        /// 'ipfs swarm disconnect' closes a connection to a peer address. The address format
        /// is an ipfs multiaddr:
        /// 
        /// ipfs swarm disconnect /ip4/104.131.131.82/tcp/4001/ipfs/QmaCpDMGvV2BGHeYERUEnRQAwe3N8SzbUtfsmvsqQLuvuJ
        /// </summary>
        /// <param name="multiAddresses">addresses of peers to disconnect from</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns>'disconnect <address> successs' on success</returns>
        public async Task<ICollection<IpfsPeerConnectionStatus>> Disconnect(IEnumerable<MultiAddress> multiAddresses, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Disconnect(multiAddresses.Select(x=>x.ToString()), cancellationToken);
        }

        /// <summary>
        /// Close connection to a given address
        /// 
        /// 'ipfs swarm disconnect' closes a connection to a peer address. The address format
        /// is an ipfs multiaddr:
        /// 
        /// ipfs swarm disconnect /ip4/104.131.131.82/tcp/4001/ipfs/QmaCpDMGvV2BGHeYERUEnRQAwe3N8SzbUtfsmvsqQLuvuJ
        /// </summary>
        /// <param name="multiAddress">address of peer to disconnect from</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns>'disconnect <address> successs' on success</returns>
        public async Task<ICollection<IpfsPeerConnectionStatus>> Disconnect(IEnumerable<string> multiAddresses, CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpContent content = await ExecuteGetAsync("disconnect", multiAddresses, null, cancellationToken);

            string json = await content.ReadAsStringAsync();

            var peerConnectionStatuses = _jsonSerializer.Deserialize<IDictionary<string, IList<string>>>(json);

            return peerConnectionStatuses
                .SelectMany(x => x.Value)
                .Select(x =>
                {
                    string[] components = x.Split(' ');
                    return new IpfsPeerConnectionStatus(components[0], new MultiHash(components[1]), String.Equals("SUCCESS", components[2].ToUpperInvariant()));
                })
               .ToArray();
        }

        /// <summary>
        /// List peers with open connections
        /// ipfs swarm peers lists the set of peers this node is connected to.
        /// </summary>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<ICollection<MultiAddress>> Peers(CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpContent content = await ExecuteGetAsync("peers", cancellationToken);
            string json = await content.ReadAsStringAsync();
                       
            dynamic results = JObject.Parse(json);
            IEnumerable<dynamic> peers = results.Peers;
            return peers.Select(p => p.Addr)
                        .Select(a => new MultiAddress(a.ToString()))
                        .ToArray();
        }
    }
}

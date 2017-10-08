using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ipfs.Json;

namespace Ipfs.Commands
{
    public class IpfsName : IpfsCommand
    {
        internal IpfsName(Uri commandUri, HttpClient httpClient, IJsonSerializer jsonSerializer) : base(commandUri, httpClient, jsonSerializer)
        {
        }

        /// <summary>
        /// Publish an object to IPNS
        /// 
        /// IPNS is a PKI namespace, where names are the hashes of public keys, and
        /// the private key enables publishing new (signed) values.In publish, the
        /// default value of<name> is your own identity public key.
        /// </summary>
        /// <param name="name">The IPNS name to publish to. Defaults to your node's peerID</param>
        /// <param name="ipfsPath">IPFS path of the obejct to be published at <name> </param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<IpfsNamePublish> Publish(string name, string ipfsPath, CancellationToken cancellationToken = default(CancellationToken))
        {
            var content = await ExecuteGetAsync("publish", new[] { name, ipfsPath }, cancellationToken);
            string json = await content.ReadAsStringAsync();
            var ret = JsonSerializer.Deserialize<Json.IpfsNamePublish>(json);
            return new IpfsNamePublish { Name = ret.Name, Value = new MultiHash(ret.Value)};
        }

        /// <summary>
        /// Gets the value currently published at an IPNS name
        /// 
        /// IPNS is a PKI namespace, where names are the hashes of public keys, and
        /// the private key enables publishing new (signed) values.In resolve, the
        /// default value of<name> is your own identity public key.
        /// </summary>
        /// <param name="name">The IPNS name to resolve. Defaults to your node's peerID.</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<string> Resolve(string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            var content = await ExecuteGetAsync("resolve", name, cancellationToken);
            string json = await content.ReadAsStringAsync();
            var resolve = JsonSerializer.Deserialize<IpfsNameResolve>(json);
            return resolve.Path;
        }
    }
}

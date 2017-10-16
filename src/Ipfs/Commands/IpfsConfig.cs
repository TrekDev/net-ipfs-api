using Ipfs.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ipfs.Commands
{
    public class IpfsConfig : IpfsCommand
    {
        internal IpfsConfig(Uri commandUri, HttpClient httpClient, IJsonSerializer jsonSerializer) : base(commandUri, httpClient, jsonSerializer)
        {
        }

        /// <summary>
        /// Opens the config file for editing in $EDITOR
        /// To use 'ipfs config edit', you must have the $EDITOR environment
        /// variable set to your preferred text editor.
        /// </summary>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<HttpContent> Edit(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await ExecuteGetAsync("edit", cancellationToken);
        }

        /// <summary>
        /// Replaces the config with <file>
        /// Make sure to back up the config file first if neccessary, this operation
        /// can't be undone.
        /// </summary>
        /// <param name="file">The file to use as the new config</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<HttpContent> Replace(string file, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await ExecuteGetAsync("replace", file, cancellationToken);
        }

        /// <summary>
        /// Outputs the content of the config file
        /// WARNING: Your private key is stored in the config file, and it will be
        /// included in the output of this command.
        /// </summary>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<IpfsConfigShow> Show(CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpContent content = await ExecuteGetAsync("show", cancellationToken);
            var json = await content.ReadAsStringAsync();
            return _jsonSerializer.Deserialize<IpfsConfigShow>(json);
        }
    }
}

using Ipfs.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ipfs.Commands
{
    public class IpfsLog : IpfsCommand
    {
        internal IpfsLog(Uri commandUri, HttpClient httpClient, IJsonSerializer jsonSerializer) : base(commandUri, httpClient, jsonSerializer)
        {
        }

        /// <summary>
        /// Change the logging level
        /// 
        /// 'ipfs log level' is a utility command used to change the logging
        /// output of a running daemon.
        /// </summary>
        /// <param name="subsystem">the subsystem logging identifier. Use 'all' for all subsystems.</param>
        /// <param name="level">one of: debug, info, notice, warning, error, critical</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns>Confirmation message</returns>
        public async Task<string> Level(string subsystem, IpfsLogLevel level, CancellationToken cancellationToken = default(CancellationToken))
        {
            string levelValue = null;

            switch (level)
            {
                case IpfsLogLevel.Debug:
                    levelValue = "debug";
                    break;
                case IpfsLogLevel.Info:
                    levelValue = "info";
                    break;
                case IpfsLogLevel.Notice:
                    levelValue = "notice";
                    break;
                case IpfsLogLevel.Warning:
                    levelValue = "warning";
                    break;
                case IpfsLogLevel.Error:
                    levelValue = "error";
                    break;
                case IpfsLogLevel.Critical:
                    levelValue = "critical";
                    break;
                default:
                    break;
            }

            HttpContent content = await ExecuteGetAsync("level", new[] { subsystem, levelValue }, cancellationToken);

            string json = await content.ReadAsStringAsync();

            if(String.IsNullOrEmpty(json))
            {
                return String.Empty;
            }

            var jsonDict = _jsonSerializer.Deserialize<Dictionary<string, string>>(json);

            return jsonDict["Message"];
        }

        /// <summary>
        /// Read the logs
        /// 'ipfs log tail' is a utility command used to read log output as it is written.
        /// </summary>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns>A stream to the log tail output</returns>
        public async Task<Stream> Tail(CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpContent content = await ExecuteGetAsync("tail", cancellationToken);

            return await content.ReadAsStreamAsync();
        }
    }
}

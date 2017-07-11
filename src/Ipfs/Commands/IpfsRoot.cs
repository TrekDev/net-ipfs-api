using Ipfs.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Ipfs.Commands
{
    public class IpfsRoot : IpfsCommand
    {
        internal IpfsRoot(Uri commandUri, HttpClient httpClient, IJsonSerializer jsonSerializer) : base(commandUri, httpClient, jsonSerializer)
        {
        }

        /// <summary>
        /// Add an object to ipfs.
        /// Adds contents of <path> to ipfs. Use -r to add directories.
        /// Note that directories are added recursively, to form the ipfs
        /// MerkleDAG.A smarter partial add with a staging area(like git)
        /// remains to be implemented
        /// </summary>
        /// <param name="stream">The ipfs stream.</param>
        /// <param name="recursive">Add directory paths recursively</param>
        /// <param name="quiet">Write minimal output</param>
        /// <param name="progress">Stream progress data</param>
        /// <param name="wrapWithDirectory">Wrap files with a directory object</param>
        /// <param name="trickle">Use trickle-dag format for dag generation</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns>The merkle node of the added file in IPFS</returns>
        public async Task<MerkleNode> Add(IpfsStream stream, bool recursive = false, bool quiet = false, bool wrapWithDirectory = false, bool trickle = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var flags = new Dictionary<string, string>()
            {
                { "stream-channels", "true" },
                { "progress", "false" }
            };

            if (recursive)
            {
                flags.Add("recursive", "true");
            }

            if (quiet)
            {
                flags.Add("quiet", "true");
            }

            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            StreamContent sc = new StreamContent(stream);
            sc.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            multiContent.Add(sc, "file", stream.Name);

            HttpContent content = await ExecutePostAsync("add", null, flags, multiContent, cancellationToken);

            string json = await content.ReadAsStringAsync();

            IpfsAdd add = _jsonSerializer.Deserialize<IpfsAdd>(json);

            return new MerkleNode(new MultiHash(add.Hash)) { Name = add.Name };
        }

        /// <summary>
        /// Show IPFS object data
        /// Retrieves the object named by <ipfs-path> and outputs the data
        /// it contains.
        /// </summary>
        /// <param name="ipfsPath">The path to the IPFS object(s) to be outputted</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns>A stream to your file</returns>
        public async Task<Stream> Cat(string ipfsPath, CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpContent content = await ExecuteGetAsync("cat", ipfsPath, cancellationToken);
            return await content.ReadAsStreamAsync();
        }

        /// <summary>
        /// List all available commands.
        /// Lists all available commands (and subcommands) and exits.
        /// </summary>
        /// <returns></returns>
        public async Task<Json.IpfsCommand> Commands(CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpContent content = await ExecuteGetAsync("commands", cancellationToken);
            string json = await content.ReadAsStringAsync();
            return _jsonSerializer.Deserialize<Json.IpfsCommand>(json);
        }

        /// <summary>
        /// get and set IPFS config values
        /// ipfs config controls configuration variables. It works
        /// much like 'git config'. The configuration values are stored in a config
        /// file inside your IPFS repository.
        /// </summary>
        /// <param name="key">The key of the config entry (e.g. "Addresses.API")</param>
        /// <param name="value">The value to set the config entry to</param>
        /// <param name="bool">Set a boolean value</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<HttpContent> ConfigCommand(string key, string value = null, bool @bool = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var args = new Dictionary<string, string>();

            if (@bool)
            {
                args.Add("bool", "true");
            }

            return await ExecuteGetAsync("config", new[] { key, value }, cancellationToken);
        }

        /// <summary>
        /// DNS link resolver
        ///
        /// Multihashes are hard to remember, but domain names are usually easy to
        /// remember.To create memorable aliases for multihashes, DNS TXT
        /// records can point to other DNS links, IPFS objects, IPNS keys, etc.
        /// This command resolves those links to the referenced object.
        ///
        /// For example, with this DNS TXT record:
        ///
        /// ipfs.io.TXT "dnslink=/ipfs/QmRzTuh2Lpuz7Gr39stNr6mTFdqAghsZec1JoUnfySUzcy ..."
        ///
        /// The resolver will give:
        ///
        /// > ipfs dns ipfs.io
        /// /ipfs/QmRzTuh2Lpuz7Gr39stNr6mTFdqAghsZec1JoUnfySUzcy
        ///
        /// And with this DNS TXT record:
        ///
        /// ipfs.ipfs.io.TXT "dnslink=/dns/ipfs.io ..."
        ///
        /// The resolver will give:
        ///
        /// > ipfs dns ipfs.io
        /// /dns/ipfs.io
        /// > ipfs dns --recursive
        /// /ipfs/QmRzTuh2Lpuz7Gr39stNr6mTFdqAghsZec1JoUnfySUzcy
        /// </summary>
        /// <param name="domainName">The domain-name name to resolve.</param>
        /// <param name="recursive">Resolve until the result is not a DNS link</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<HttpContent> Dns(string domainName, bool recursive = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var flags = new Dictionary<string, string>();

            if(recursive)
            {
                flags.Add("recursive", "true");
            }

            return await ExecuteGetAsync("dns", domainName, flags, cancellationToken);
        }

        /// <summary>
        /// Download IPFS objects
        ///
        /// Retrieves the object named by <ipfs-path> and stores the data to disk.
        ///
        /// By default, the output will be stored at./<ipfs-path>, but an alternate path
        ///
        /// can be specified with '--output=<path>' or '-o=<path>'.
        ///
        /// To output a TAR archive instead of unpacked files, use '--archive' or '-a'.
        ///
        /// To compress the output with GZIP compression, use '--compress' or '-C'. You
        /// may also specify the level of compression by specifying '-l=<1-9>'.
        /// </summary>
        /// <param name="ipfsPath">The path to the IPFS object(s) to be outputted</param>
        /// <param name="output">The path where output should be stored</param>
        /// <param name="archive">Output a TAR archive</param>
        /// <param name="compress">Compress the output with GZIP compression</param>
        /// <param name="compressionLevel">The level of compression (1-9)</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<HttpContent> Get(string ipfsPath, string output = null, bool archive = false, bool compress = false, int? compressionLevel = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var flags = new Dictionary<string, string>();

            if (output != null)
            {
                flags.Add("output", output);
            }

            if (archive)
            {
                flags.Add("archive", "true");
            }

            if (compress)
            {
                flags.Add("compress", "true");
            }

            if (compressionLevel != null)
            {
                flags.Add("compressionLevel", compressionLevel.Value.ToString());
            }

            return await ExecuteGetAsync("get", ipfsPath, flags, cancellationToken);
        }

        /// <summary>
        /// Show IPFS Node ID info
        ///
        /// Prints out information about the specified peer,
        /// if no peer is specified, prints out local peers info.
        ///
        /// ipfs id supports the format option for output with the following keys:
        /// <id> : the peers id
        /// <aver>: agent version
        /// <pver>: protocol version
        /// <pubkey>: public key
        /// </summary>
        /// <param name="peerId">peer.ID of node to look up</param>
        /// <param name="format">optional output format</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<IpfsID> Id(string peerId = null, string format = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var flags = new Dictionary<string, string>();

            if (format != null)
            {
                flags.Add("format", format);
            }

            HttpContent content = await ExecuteGetAsync("id", peerId, flags, cancellationToken);

            string json = await content.ReadAsStringAsync();

            Json.IpfsID id = _jsonSerializer.Deserialize<Json.IpfsID>(json);

            return new IpfsID
            {
                ID = new MultiHash(id.ID),
                PublicKey = id.PublicKey,
                Addresses = id.Addresses.Select(x=>new MultiAddress(x)).ToList(),
                AgentVersion = id.AgentVersion,
                ProtocolVersion = id.ProtocolVersion
            };
        }

        /// <summary>
        /// List links from an object.
        ///
        ///  Retrieves the object named by <ipfs-path> and displays the links
        ///  it contains, with the following format:
        ///
        /// <link base58 hash> <link size in bytes> <link name>
        /// </summary>
        /// <param name="path">The path to the IPFS object(s) to list links from</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<IList<MerkleNode>> Ls(string path, CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpContent content = await ExecuteGetAsync("ls", path, cancellationToken);
            string json = await content.ReadAsStringAsync();
            var jsonDict = _jsonSerializer.Deserialize<IDictionary<string, IList<MerkleNode>>>(json);
            return jsonDict.Values.First();
        }

        /// <summary>
        /// Mounts IPFS to the filesystem (read-only)
        ///
        /// Mount ipfs at a read-only mountpoint on the OS (default: /ipfs and /ipns).
        /// All ipfs objects will be accessible under that directory.Note that the
        /// root will not be listable, as it is virtual. Access known paths directly.
        ///
        /// You may have to create /ipfs and /ipfs before using 'ipfs mount'
        /// </summary>
        /// <param name="f">The path where IPFS should be mounted</param>
        /// <param name="n">The path where IPNS should be mounted</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<HttpContent> Mount(string f = null, string n = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var flags = new Dictionary<string, string>();

            if (f != null)
            {
                flags.Add("f", f);
            }

            if (n != null)
            {
                flags.Add("n", n);
            }

            return await ExecuteGetAsync("mount", cancellationToken);
        }

        /// <summary>
        /// send echo request packets to IPFS hosts
        ///
        /// ipfs ping is a tool to test sending data to other nodes. It finds nodes
        /// via the routing system, send pings, wait for pongs, and print out round-
        /// trip latency information.
        /// </summary>
        /// <param name="peerId">ID of peer to be pinged</param>
        /// <param name="count">number of ping messages to send</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<IpfsPingResult> Ping(string peerId, int? count = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpContent content = await ExecuteGetAsync("ping", peerId, cancellationToken);
            string json = await content.ReadAsStringAsync();
            return _jsonSerializer.Deserialize<IpfsPingResult>(json);
        }

        /// <summary>
        /// Lists links (references) from an object
        /// Retrieves the object named by <ipfs-path> and displays the link
        /// hashes it contains, with the following format:
        ///  <link base58 hash>
        /// </summary>
        /// <param name="ipfsPath">Path to the object(s) to list refs from</param>
        /// <param name="format">Emit edges with given format. tokens: <src> <dst> <linkname></param>
        /// <param name="edges">Emit edge format: `<from> -> <to>`</param>
        /// <param name="unique">Omit duplicate refs from output</param>
        /// <param name="recursive">Recursively list links of child nodes</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<HttpContent> RefsCommand(string ipfsPath, string format = null, bool edges = false, bool unique = false, bool recursive = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var flags = new Dictionary<string, string>();

            if (format != null)
            {
                flags.Add("format", format);
            }

            if (edges)
            {
                flags.Add("edges", "true");
            }

            if (unique)
            {
                flags.Add("unique", "true");
            }

            if (recursive)
            {
                flags.Add("recursive", "true");
            }

            return await ExecuteGetAsync("refs", ipfsPath, cancellationToken);
        }

        /// <summary>
        /// Shuts down the IPFS daemon
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <remarks>Currently there is no way to determine whether or not the command succeeded
        ///          since the daemon shuts down before sending a response</remarks>
        public async Task Shutdown(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                await ExecuteGetAsync("shutdown", cancellationToken);
            }
            catch (HttpRequestException)
            {                
                /* Currently shutdown always results in a HttpRequestException 
                 * because the daemon shuts down before sending a response */
            }
        }

        /// <summary>
        /// An introduction to IPFS
        ///
        /// This is a tour that takes you through various IPFS concepts,
        /// features, and tools to make sure you get up to speed with
        /// IPFS very quickly
        /// </summary>
        /// <param name="id">The id of the topic you would like to tour</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns></returns>
        public async Task<Stream> TourCommand(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpContent content = await ExecuteGetAsync("tour", id, cancellationToken);
            return await content.ReadAsStreamAsync();
        }

        /// <summary>
        /// ipfs version - Shows ipfs version information
        /// </summary>
        /// <param name="number">Only show the version number</param>
        /// <param name="cancellationToken">Token allowing you to cancel the request</param>
        /// <returns>Returns the current version of ipfs and exits.</returns>
        public async Task<IpfsVersion> Version(bool number = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpContent content = await ExecuteGetAsync("version", cancellationToken);
            string json = await content.ReadAsStringAsync();
            return _jsonSerializer.Deserialize<IpfsVersion>(json);
        }
    }
}

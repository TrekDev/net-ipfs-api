using Ipfs.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ipfs.Commands
{
    public class IpfsObject : IpfsCommand
    {
        internal IpfsObject(Uri commandUri, HttpClient httpClient, IJsonSerializer jsonSerializer) : base(commandUri, httpClient, jsonSerializer)
        {
        }

        /// <summary>
        /// Outputs the raw bytes in an IPFS object
        /// 
        /// ipfs data is a plumbing command for retreiving the raw bytes stored in
        /// a DAG node.It outputs to stdout, and <key> is a base58 encoded
        /// multihash.
        /// </summary>
        /// <param name="key">Key of the object to retrieve, in base58-encoded multihash format</param>
        /// <returns></returns>
        public async Task<HttpContent> Data(string key)
        {
            return await ExecuteGetAsync("data", key);
        }

        /// <summary>
        /// Get and serialize the DAG node named by <key>
        /// 'ipfs object get' is a plumbing command for retreiving DAG nodes.
        /// It serializes the DAG node to the format specified by the "--encoding"
        /// flag.It outputs to stdout, and <key> is a base58 encoded multihash.
        /// </summary>
        /// <param name="key">Key of the object to retrieve (in base58-encoded multihash format)</param>
        /// <param name="encoding">The encoding of the data</param>
        /// <returns></returns>
        public async Task<HttpContent> Get(string key, IpfsEncoding encoding)
        {
            var flags = new Dictionary<string, string>();

            flags.Add("encoding", GetIpfsEncodingValue(encoding));

            return await ExecuteGetAsync("get", key, flags);
        }

        /// <summary>
        /// Outputs the links pointed to by the specified object
        /// 'ipfs object links' is a plumbing command for retreiving the links from
        /// a DAG node.It outputs to stdout, and <key> is a base58 encoded
        /// multihash.
        /// </summary>
        /// <param name="key">Key of the object to retrieve, in base58-encoded multihash format</param>
        /// <returns></returns>
        public async Task<IpfsObjectLinks> Links(string key)
        {
            HttpContent content = await ExecuteGetAsync("links", key);

            string json = await content.ReadAsStringAsync();

            Json.IpfsObjectLinks links = _jsonSerializer.Deserialize<Json.IpfsObjectLinks>(json);

            return new IpfsObjectLinks
            {
                Hash = new MultiHash(links.Hash),
                Links = links.Links == null ? null :
                        links.Links.Select(x => new Link
                        {
                            Hash = new MultiHash(x.Hash),
                            Name = x.Name,
                            Size = x.Size
                        }).ToList()
            };
        }

        /// <summary>
        /// Stores input as a DAG object, outputs its key
        /// 'ipfs object put' is a plumbing command for storing DAG nodes.
        /// It reads from stdin, and the output is a base58 encoded multihash.
        /// </summary>
        /// <param name="data">Data to be stored as a DAG object</param>
        /// <param name="encoding">Encoding type of <data>, either "protobuf" or "json"</param>
        /// <returns></returns>
        public async Task<HttpContent> Put(string data, IpfsEncoding encoding)
        {
            var flags = new Dictionary<string, string>();

            flags.Add("encoding", GetIpfsEncodingValue(encoding));

            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            StringContent sc = new StringContent(data);
            sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            multiContent.Add(sc, "file", "file");

            return await ExecutePostAsync("put", null, flags, multiContent);
        }

        /// <summary>
        /// Get stats for the DAG node named by <key>
        /// 'ipfs object stat' is a plumbing command to print DAG node statistics.
        /// <key> is a base58 encoded multihash.It outputs to stdout:
        /// </summary>
        /// <param name="key">Key of the object to retrieve (in base58-encoded multihash format)</param>
        /// <returns></returns>
        public async Task<IpfsObjectStat> Stat(string key)
        {
            HttpContent content = await ExecuteGetAsync("stat", key);

            string json = await content.ReadAsStringAsync();

            Json.IpfsObjectStat ret = _jsonSerializer.Deserialize<Json.IpfsObjectStat>(json);

            return new IpfsObjectStat
            {
                Hash = new MultiHash(ret.Hash),
                BlockSize = ret.BlockSize,
                CumulativeSize = ret.CumulativeSize,
                DataSize = ret.DataSize,
                LinksSize = ret.LinksSize,
                NumLinks = ret.NumLinks
            };
        }

        private string GetIpfsEncodingValue(IpfsEncoding encoding)
        {
            switch (encoding)
            {
                case IpfsEncoding.Json:
                    return "json";
                case IpfsEncoding.Protobuf:
                    return "protobuf";
                default:
                    return null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Ipfs.Json;

namespace Ipfs.Commands
{
    public class IpfsTar : IpfsCommand
    {
        internal IpfsTar(Uri commandUri, HttpClient httpClient, IJsonSerializer jsonSerializer) : base(commandUri, httpClient, jsonSerializer)
        {
        }

        //public async Task<HttpContent> Add()
        //{

        //}

        //public async Task<HttpContent> Cat()
        //{

        //}
    }
}

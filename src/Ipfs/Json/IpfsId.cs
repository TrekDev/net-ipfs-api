using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ipfs.Json
{
    public class IpfsID
    {
        public string ID { get; set; }
        public string PublicKey { get; set; }
        public List<string> Addresses { get; set; }
        public string AgentVersion { get; set; }
        public string ProtocolVersion { get; set; }
    }
}

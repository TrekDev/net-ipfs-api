using System.Collections.Generic;

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

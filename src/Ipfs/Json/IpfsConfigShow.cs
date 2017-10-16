using System.Collections.Generic;

namespace Ipfs.Json
{
    public class Identity
    {
        public string PeerID { get; set; }
        public string PrivKey { get; set; }
    }

    public class Datastore
    {
        public string Type { get; set; }
        public string Path { get; set; }
        public string StorageMax { get; set; }
        public int StorageGCWatermark { get; set; }
        public string GCPeriod { get; set; }
    }

    public class Addresses
    {
        public List<string> Swarm { get; set; }
        public string API { get; set; }
        public string Gateway { get; set; }
    }

    public class Mounts
    {
        public string IPFS { get; set; }
        public string IPNS { get; set; }
        public bool FuseAllowOther { get; set; }
    }

    public class Version
    {
        public string Current { get; set; }
        public string Check { get; set; }
        public string CheckDate { get; set; }
        public string CheckPeriod { get; set; }
        public string AutoUpdate { get; set; }
    }

    public class MDNS
    {
        public bool Enabled { get; set; }
        public int Interval { get; set; }
    }

    public class Discovery
    {
        public MDNS MDNS { get; set; }
    }

    public class Ipns
    {
        public string RepublishPeriod { get; set; }
        public string RecordLifetime { get; set; }
        public int ResolveCacheSize { get; set; }
    }

    public class Tour
    {
        public string Last { get; set; }
    }

    public class Gateway
    {
        public object HTTPHeaders { get; set; }
        public string RootRedirect { get; set; }
        public bool Writable { get; set; }
    }

    public class SupernodeRouting
    {
        public List<string> Servers { get; set; }
    }

    public class API
    {
        public object HTTPHeaders { get; set; }
    }

    public class Swarm
    {
        public object AddrFilters { get; set; }
    }

    public class IpfsConfigShow
    {
        public Identity Identity { get; set; }
        public Datastore Datastore { get; set; }
        public Addresses Addresses { get; set; }
        public Mounts Mounts { get; set; }
        public Version Version { get; set; }
        public Discovery Discovery { get; set; }
        public Ipns Ipns { get; set; }
        public List<string> Bootstrap { get; set; }
        public Tour Tour { get; set; }
        public Gateway Gateway { get; set; }
        public SupernodeRouting SupernodeRouting { get; set; }
        public API API { get; set; }
        public Swarm Swarm { get; set; }
    }
}

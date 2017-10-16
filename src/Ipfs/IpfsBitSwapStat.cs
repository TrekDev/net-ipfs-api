using System;
using System.Collections.Generic;

namespace Ipfs
{
    public class IpfsBitSwapStat : IEquatable<IpfsBitSwapStat>
    {
        public int ProvideBufLen { get; set; }

        public object Wantlist { get; set; }

        public List<MultiHash> Peers { get; set; }

        public int BlocksReceived { get; set; }

        public int DupBlksReceived { get; set; }

        public int DupDataReceived { get; set; }

        public bool Equals(IpfsBitSwapStat other)
        {
            return Equals(other.ProvideBufLen, ProvideBufLen)
                && Equals(other.Wantlist, Wantlist)
                && Equals(other.Peers, Peers)
                && Equals(other.BlocksReceived, BlocksReceived)
                && Equals(other.DupBlksReceived, DupBlksReceived)
                && Equals(other.DupDataReceived, DupDataReceived);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this)) return true;

            var other = obj as IpfsBitSwapStat;

            if (other == null) return false;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            return ProvideBufLen.GetHashCode()
                + Wantlist.GetHashCode()
                + Peers.GetHashCode()
                + BlocksReceived.GetHashCode()
                + DupBlksReceived.GetHashCode()
                + DupDataReceived.GetHashCode();
        }
    }
}

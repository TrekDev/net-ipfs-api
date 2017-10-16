using System;
using System.Collections.Generic;
using System.Linq;

namespace Ipfs
{
    public class IpfsPeer : IEquatable<IpfsPeer>
    {
        public IpfsPeer(MultiHash peerId, IEnumerable<MultiAddress> addresses)
        {
            PeerId = peerId;
            Addresses = addresses.ToList();
        }

        public IList<MultiAddress> Addresses { get; private set; }

        public MultiHash PeerId { get; private set; }

        public bool Equals(IpfsPeer other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals(other.Addresses, Addresses)
                && Equals(other.PeerId, PeerId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
            {
                return true;
            }

            var other = obj as IpfsPeer;

            return Equals(other, this);
        }

        public override int GetHashCode()
        {
            return PeerId.GetHashCode() + Addresses.GetHashCode();
        }

        public override string ToString()
        {
            return PeerId.ToString();
        }
    }
}

using System;
using System.Collections.Generic;

namespace Ipfs
{
    public class MerkleNode : IEquatable<MerkleNode>
    {
        public MerkleNode(string multiHash) : this(new MultiHash(multiHash))
        { }

        public MerkleNode(MultiHash multiHash)
        {
            Hash = multiHash;
        }

        public MerkleNode()
        {

        }

        public MultiHash Hash { get; set; }

        public string Name { get; set; }

        public long? Size { get; set; }

        public int? Type { get; set; }

        public byte[] Data { get; set; }

        public IEnumerable<MerkleNode> Links { get; set; }

        public bool Equals(MerkleNode other)
        {
            return Equals(other.Hash, Hash)
                && Equals(other.Name, Name)
                && Equals(other.Size, Size)
                && Equals(other.Type, Type)
                && Equals(other.Data, Data)
                && Equals(other.Links, Links);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if(!(obj is MerkleNode other))
            {
                return false;
            }

            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Hash.GetHashCode();
        }

        public override string ToString()
        {
            return Hash.ToString();
        }
    }
}

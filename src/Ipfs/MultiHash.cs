using System;
using System.Collections.Generic;
using System.Linq;

namespace Ipfs
{
    public class MultiHash : IEquatable<MultiHash>
    {
        public MultiHash(string base58) : this(Base58.Decode(base58)) { }

        public MultiHash(byte[] value)
        {
            if(value.Length < 2)
            {
                throw new ArgumentOutOfRangeException("value");
            }

            Value = value;
        }

        public MultiHash(byte fnCode, byte size, byte[] digest)
        {
            List<byte> value = new List<byte>
            {
                fnCode,
                size
            };

            value.AddRange(digest);

            Value = value.ToArray();
        }

        public byte[] Value { get; private set; }

        public byte FnCode
        {
            get { return Value[0]; }
        }

        public byte DigestSize
        {
            get { return Value[1]; }
        }

        public byte[] HashDigest
        {
            get { return Value.Skip(2).ToArray(); }
        }

        public bool Equals(MultiHash other)
        {
            if (other == null) return false;

            return Equals(other.Value, Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this)) return true;

            var other = obj as MultiHash;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static explicit operator string(MultiHash multiHash)
        {
            return multiHash.ToString();
        }

        public override string ToString()
        {
            return Base58.Encode(Value);
        }
    }
}

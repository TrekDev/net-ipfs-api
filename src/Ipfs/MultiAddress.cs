using System;

namespace Ipfs
{
    public class MultiAddress : IEquatable<MultiAddress>
    {
        private readonly string[] _components;

        public MultiAddress(string uriString)
        {
            OriginalString = uriString;
            _components = OriginalString.Split('/');
        }

        public string OriginalString { get; private set; }

        public string Version => _components[1];

        public string Address => _components[2];

        public string Protocol => _components[3];

        public string Port => _components[4];

        public string Application => _components[5];

        public string Resource => _components[6];

        public bool Equals(MultiAddress other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals(other.OriginalString, OriginalString);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, this))
            {
                return true;
            }

            var other = obj as MultiAddress;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            return OriginalString.GetHashCode();
        }

        public static explicit operator string(MultiAddress multiAddress)
        {
            return multiAddress.ToString();
        }

        public override string ToString()
        {
            return OriginalString;
        }
    }
}

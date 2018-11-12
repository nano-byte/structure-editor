// Copyright Bastian Eicher
// Licensed under the MIT License

using System;

namespace NanoByte.StructureEditor.Sample.Model
{
    public class Address : IEquatable<Address>
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public override string ToString() => Street + ", " + City + ", " + Country;

        public bool Equals(Address other)
            => other != null
            && Street == other.Street
            && City == other.City
            && Country == other.Country;

        public override bool Equals(object obj)
            => obj is Address other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Street?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (City?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Country?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}

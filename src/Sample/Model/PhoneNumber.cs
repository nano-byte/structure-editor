// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.StructureEditor.Sample.Model
{
    /// <summary>
    /// Common base class for various types of phone numbers.
    /// </summary>
    public abstract class PhoneNumber
    {
        public string? CountryCode { get; set; }

        public string? AreaCode { get; set; }

        public string? LocalNumber { get; set; }

        public override string ToString() => CountryCode + " " + AreaCode + " " + LocalNumber;

        protected bool Equals(PhoneNumber other)
            => other != null
            && CountryCode == other.CountryCode
            && AreaCode == other.AreaCode
            && LocalNumber == other.LocalNumber;

        public override bool Equals(object? obj)
            => obj != null
            && obj.GetType() == GetType()
            && Equals((PhoneNumber)obj);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CountryCode?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (AreaCode?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (LocalNumber?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}

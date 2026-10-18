// Copyright Bastian Eicher
// Licensed under the MIT License

#if NET
using System.Text.Json.Serialization;
#endif

namespace NanoByte.StructureEditor.Sample.Model;

/// <summary>
/// Common base class for various types of phone numbers.
/// </summary>
#if NET
[JsonPolymorphic]
[JsonDerivedType(typeof(LandlineNumber), nameof(LandlineNumber))]
[JsonDerivedType(typeof(MobileNumber), nameof(MobileNumber))]
#endif
public abstract class PhoneNumber
{
    public string? CountryCode { get; set; }

    public string? AreaCode { get; set; }

    public string? LocalNumber { get; set; }

    public override string ToString() => CountryCode + " " + AreaCode + " " + LocalNumber;
}

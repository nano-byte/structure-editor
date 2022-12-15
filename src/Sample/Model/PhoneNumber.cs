// Copyright Bastian Eicher
// Licensed under the MIT License

using Generator.Equals;

namespace NanoByte.StructureEditor.Sample.Model;

/// <summary>
/// Common base class for various types of phone numbers.
/// </summary>
[Equatable]
public abstract partial class PhoneNumber
{
    public string? CountryCode { get; set; }

    public string? AreaCode { get; set; }

    public string? LocalNumber { get; set; }

    public override string ToString() => CountryCode + " " + AreaCode + " " + LocalNumber;
}

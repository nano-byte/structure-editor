// Copyright Bastian Eicher
// Licensed under the MIT License

using System.ComponentModel;
using Generator.Equals;

namespace NanoByte.StructureEditor.Sample.Model;

/// <summary>
/// A postal address.
/// </summary>
[Description("A postal address.")]
[Equatable]
public partial class Address
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    public override string ToString() => Street + ", " + City + ", " + Country;
}

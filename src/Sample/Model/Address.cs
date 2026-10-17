// Copyright Bastian Eicher
// Licensed under the MIT License

using System.ComponentModel;

namespace NanoByte.StructureEditor.Sample.Model;

/// <summary>
/// A postal address.
/// </summary>
[Description("A postal address.")]
public class Address
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    public override string ToString() => Street + ", " + City + ", " + Country;
}

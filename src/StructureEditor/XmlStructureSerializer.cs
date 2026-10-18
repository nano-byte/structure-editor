// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using System.Xml;
using NanoByte.Common;
using NanoByte.Common.Storage;

namespace NanoByte.StructureEditor;

/// <summary>
/// Serializes nodes as XML using <see cref="System.Xml.Serialization.XmlSerializer"/>.
/// </summary>
public class XmlStructureSerializer : IStructureSerializer
{
    /// <summary>
    /// A shared instance of the serializer.
    /// </summary>
    public static XmlStructureSerializer Instance { get; } = new();

    /// <inheritdoc/>
    public string FormatName => "XML";

    /// <inheritdoc/>
    public string Serialize<T>(T value) where T : class
        => value.ToXmlString()
                // Trim off <?xml> header
               .GetRightPartAtFirstOccurrence('\n');

    /// <inheritdoc/>
    public T Deserialize<T>(string text) where T : class
        => XmlStorage.FromXmlString<T>(text);

    /// <inheritdoc/>
    public bool TryGetErrorLocation(Exception exception, out int line, out int column, out string message)
    {
        if (exception is InvalidDataException {InnerException: XmlException xmlException})
        {
            line = xmlException.LineNumber;
            column = xmlException.LinePosition;
            message = xmlException.Message;
            return true;
        }

        line = column = 0;
        message = "";
        return false;
    }
}

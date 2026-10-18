// Copyright Bastian Eicher
// Licensed under the MIT License

#if !NET45
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NanoByte.StructureEditor;

/// <summary>
/// Serializes nodes as JSON using <see cref="JsonSerializer"/>.
/// </summary>
/// <param name="options">Options for the serializer. Use <see cref="DefaultOptions"/> as a starting point for customization.</param>
public class JsonStructureSerializer(JsonSerializerOptions options) : IStructureSerializer
{
    /// <summary>
    /// Creates a serializer with <see cref="DefaultOptions"/>.
    /// </summary>
    public JsonStructureSerializer() : this(DefaultOptions) {}

    /// <summary>
    /// The default options for the serializer: indented output without unnecessary escaping and populating existing (e.g., get-only) collections and objects instead of replacing them.
    /// </summary>
    public static JsonSerializerOptions DefaultOptions => new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Output is edited by humans, not embedded in HTML
        PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate
    };

    /// <summary>
    /// A shared instance of the serializer using <see cref="DefaultOptions"/>.
    /// </summary>
    public static JsonStructureSerializer Instance { get; } = new();

    /// <inheritdoc/>
    public string FormatName => "JSON";

    /// <inheritdoc/>
    public string Serialize<T>(T value) where T : class
        => JsonSerializer.Serialize(value, options);

    /// <inheritdoc/>
    public T Deserialize<T>(string text) where T : class
        => JsonSerializer.Deserialize<T>(text, options)
        ?? throw new InvalidDataException($"Expected a JSON object representing {typeof(T).Name} but got null.");

    /// <inheritdoc/>
    public bool TryGetErrorLocation(Exception exception, out int line, out int column, out string message)
    {
        if (exception is JsonException {LineNumber: {} lineNumber} jsonException)
        {
            line = (int)lineNumber + 1;
            column = (int)(jsonException.BytePositionInLine ?? 0) + 1;
            message = jsonException.Message;
            return true;
        }

        line = column = 0;
        message = "";
        return false;
    }
}
#endif

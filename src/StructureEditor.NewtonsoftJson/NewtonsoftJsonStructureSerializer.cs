// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using Newtonsoft.Json;

namespace NanoByte.StructureEditor.NewtonsoftJson;

/// <summary>
/// Serializes nodes as JSON using Newtonsoft.Json.
/// </summary>
/// <param name="settings">Settings for the serializer. Use <see cref="DefaultSettings"/> as a starting point for customization.</param>
public class NewtonsoftJsonStructureSerializer(JsonSerializerSettings settings) : IStructureSerializer
{
    /// <summary>
    /// Creates a serializer with <see cref="DefaultSettings"/>.
    /// </summary>
    public NewtonsoftJsonStructureSerializer() : this(DefaultSettings) {}

    /// <summary>
    /// The default settings for the serializer: indented output.
    /// </summary>
    public static JsonSerializerSettings DefaultSettings => new()
    {
        Formatting = Formatting.Indented
    };

    /// <summary>
    /// A shared instance of the serializer using <see cref="DefaultSettings"/>.
    /// </summary>
    public static NewtonsoftJsonStructureSerializer Instance { get; } = new();

    /// <inheritdoc/>
    public string FormatName => "JSON";

    /// <inheritdoc/>
    public string Serialize<T>(T value) where T : class
        => JsonConvert.SerializeObject(value, typeof(T), settings);

    /// <inheritdoc/>
    public T Deserialize<T>(string text) where T : class
        => JsonConvert.DeserializeObject<T>(text, settings)
        ?? throw new InvalidDataException($"Expected a JSON object representing {typeof(T).Name} but got null.");

    /// <inheritdoc/>
    public bool TryGetErrorLocation(Exception exception, out int line, out int column, out string message)
    {
        switch (exception)
        {
            case JsonReaderException {LineNumber: > 0} readerException:
                line = readerException.LineNumber;
                column = readerException.LinePosition;
                message = readerException.Message;
                return true;
            case JsonSerializationException {LineNumber: > 0} serializationException:
                line = serializationException.LineNumber;
                column = serializationException.LinePosition;
                message = serializationException.Message;
                return true;
            default:
                line = column = 0;
                message = "";
                return false;
        }
    }
}

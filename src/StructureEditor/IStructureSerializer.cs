// Copyright Bastian Eicher
// Licensed under the MIT License

using System;

namespace NanoByte.StructureEditor;

/// <summary>
/// Converts nodes in the structure to and from a textual representation (e.g., XML or JSON).
/// </summary>
public interface IStructureSerializer
{
    /// <summary>
    /// The name of the format (e.g., XML or JSON). Used to determine syntax highlighting.
    /// </summary>
    string FormatName { get; }

    /// <summary>
    /// Serializes an object to a string.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <typeparam name="T">The type to serialize the object as.</typeparam>
    string Serialize<T>(T value) where T : class;

    /// <summary>
    /// Deserializes an object from a string.
    /// </summary>
    /// <param name="text">The serialized representation of the object.</param>
    /// <typeparam name="T">The type of object to deserialize.</typeparam>
    /// <exception cref="Exception">The <paramref name="text"/> is not a valid serialized representation of <typeparamref name="T"/>.</exception>
    T Deserialize<T>(string text) where T : class;

    /// <summary>
    /// Extracts the location of an error from an exception thrown by <see cref="Deserialize{T}"/>.
    /// </summary>
    /// <param name="exception">The exception thrown by <see cref="Deserialize{T}"/>.</param>
    /// <param name="line">The 1-based line number of the error.</param>
    /// <param name="column">The 1-based column number of the error.</param>
    /// <param name="message">A message describing the error.</param>
    /// <returns><c>true</c> if the location could be determined; <c>false</c> otherwise.</returns>
    bool TryGetErrorLocation(Exception exception, out int line, out int column, out string message);
}

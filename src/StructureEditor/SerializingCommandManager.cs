// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using System.Text;
using NanoByte.Common.Storage;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor;

/// <summary>
/// A command manager that loads and saves its target using an <see cref="IStructureSerializer"/> instead of always using XML.
/// </summary>
/// <param name="target">The object being edited.</param>
/// <param name="serializer">The serializer used to save the <paramref name="target"/>.</param>
/// <param name="path">The path of the file the <paramref name="target"/> was loaded from. <c>null</c> if none.</param>
/// <typeparam name="T">The type of object being edited.</typeparam>
public class SerializingCommandManager<T>(T target, IStructureSerializer serializer, string? path = null) : CommandManager<T>(target, path)
    where T : class
{
    /// <summary>
    /// The serializer used to save the <see cref="CommandManager{T}.Target"/>.
    /// </summary>
    public IStructureSerializer Serializer { get; } = serializer;

    /// <inheritdoc/>
    public override void Save(string path)
    {
        if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
        if (Target == null) return;

        using (var atomic = new AtomicWrite(path))
        {
            File.WriteAllText(atomic.WritePath, Serializer.Serialize(Target), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            atomic.Commit();
        }
        Path = path;
        ClearUndo();
    }

    /// <summary>
    /// Loads an object from a file.
    /// </summary>
    /// <param name="path">The file to load from.</param>
    /// <param name="serializer">The serializer used to load and later save the object.</param>
    /// <exception cref="IOException">A problem occurs while reading the file.</exception>
    /// <exception cref="UnauthorizedAccessException">Read access to the file is not permitted.</exception>
    /// <exception cref="Exception">The <paramref name="serializer"/> failed to deserialize the file's content.</exception>
    public static SerializingCommandManager<T> Load(string path, IStructureSerializer serializer)
        => new(serializer.Deserialize<T>(File.ReadAllText(path, Encoding.UTF8)), serializer, path);
}

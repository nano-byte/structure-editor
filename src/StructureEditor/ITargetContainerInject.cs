// Copyright Bastian Eicher
// Licensed under the MIT License

using System.ComponentModel;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// Implement this interface in addition to <see cref="INodeEditor{T}"/> in order to get the target's container injected.
    /// </summary>
    /// <typeparam name="T">The type of the container.</typeparam>
    public interface ITargetContainerInject<T>
    {
        /// <summary>
        /// The <see cref="INodeEditor{T}.Target"/>'s container.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        T TargetContainer { get; set; }
    }
}

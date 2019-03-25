// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;
using System.Linq;
using NanoByte.Common.Undo;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// Describes an object that contains nodes (properties and/or lists). Provides information about how to edit this content.
    /// </summary>
    /// <typeparam name="TContainer">The type of the container to be described.</typeparam>
    public partial class ContainerDescription<TContainer> : IContainerDescription<TContainer>
        where TContainer : class
    {
        private readonly List<DescriptionBase> _descriptions = new List<DescriptionBase>();

        /// <inheritdoc/>
        public IEnumerable<Node> GetNodesIn(TContainer container)
            => _descriptions.SelectMany(description => description.GetNodesIn(container));

        /// <inheritdoc/>
        public IEnumerable<NodeCandidate> GetCandidatesFor(TContainer container)
            => _descriptions.SelectMany(description => description.GetCandidatesFor(container));

        private abstract class DescriptionBase
        {
            public abstract IEnumerable<Node> GetNodesIn(TContainer container);
            public abstract IEnumerable<NodeCandidate> GetCandidatesFor(TContainer container);
        }

        private static TEditor CreateEditor<TEditor, TElement>(TContainer container, TElement value, ICommandExecutor executor)
            where TEditor : INodeEditor<TElement>, new()
        {
            var editor = new TEditor {Target = value, CommandExecutor = executor};

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (editor is ITargetContainerInject<TContainer> inject)
                inject.TargetContainer = container;

            return editor;
        }
    }
}

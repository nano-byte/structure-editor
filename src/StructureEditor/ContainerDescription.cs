// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;
using System.Linq;

namespace NanoByte.StructureEditor
{
    /// <summary>
    /// Describes an object that contains properties and/or lists. Provides information about how to edit this content.
    /// </summary>
    /// <typeparam name="TContainer">The type of the container to be described.</typeparam>
    public partial class ContainerDescription<TContainer> : IContainerDescription<TContainer>
        where TContainer : class
    {
        private readonly List<DescriptionBase> _descriptions = new List<DescriptionBase>();

        /// <inheritdoc/>
        public IEnumerable<EntryInfo> GetEntriesIn(TContainer container)
            => _descriptions.SelectMany(description => description.GetEntriesIn(container));

        /// <inheritdoc/>
        public IEnumerable<ChildInfo> GetPossibleChildrenFor(TContainer container)
            => _descriptions.SelectMany(description => description.GetPossibleChildrenFor(container));

        private abstract class DescriptionBase
        {
            public abstract IEnumerable<EntryInfo> GetEntriesIn(TContainer container);
            public abstract IEnumerable<ChildInfo> GetPossibleChildrenFor(TContainer container);
        }
    }
}

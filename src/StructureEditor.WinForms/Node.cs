// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Windows.Forms;

namespace NanoByte.StructureEditor.WinForms
{
    internal sealed class Node : TreeNode
    {
        public EntryInfo Entry { get; }

        public Node(EntryInfo entry, TreeNode[] children)
            : base(entry.ToString(), children)
        {
            Entry = entry;
            ToolTipText = entry.Description;
            ContextMenu = new ContextMenu(new MenuItem[] {});
        }
    }
}

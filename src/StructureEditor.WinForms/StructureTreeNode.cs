// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Windows.Forms;

namespace NanoByte.StructureEditor.WinForms;

internal sealed class StructureTreeNode : TreeNode
{
    public Node Node { get; }

    public StructureTreeNode(Node node, TreeNode[] children)
        : base(node.ToString(), children)
    {
        Node = node;
        ToolTipText = node.Description;
        ContextMenuStrip = new ContextMenuStrip();
    }
}

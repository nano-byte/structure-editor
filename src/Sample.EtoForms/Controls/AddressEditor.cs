// Copyright Bastian Eicher
// Licensed under the MIT License

using Eto.Forms;
using NanoByte.Common;
using NanoByte.StructureEditor.EtoForms;
using NanoByte.StructureEditor.Sample.Model;

namespace NanoByte.StructureEditor.Sample.Controls;

/// <summary>
/// Editor for <see cref="Address"/> nodes in <see cref="AddressBook"/> structures.
/// </summary>
public class AddressEditor : NodeEditorBase<Address>
{
    public AddressEditor()
    {
        Content = new TableLayout
        {
            Spacing = new(8, 4),
            Rows =
            {
                new TableRow(new Label {Text = "Street"}, new TableCell(Bind(new TextArea {Height = 50}, PropertyPointer.ForNullable(() => Target!.Street)), scaleWidth: true)),
                new TableRow(new Label {Text = "City"}, Bind(new TextBox(), PropertyPointer.ForNullable(() => Target!.City))),
                new TableRow(new Label {Text = "Country"}, Bind(new TextBox(), PropertyPointer.ForNullable(() => Target!.Country))),
                new TableRow {ScaleHeight = true}
            }
        };
    }
}

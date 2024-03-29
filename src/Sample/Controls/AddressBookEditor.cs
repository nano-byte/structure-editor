// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Windows.Forms;
using NanoByte.Common;
using NanoByte.Common.Controls;
using NanoByte.Common.Undo;
using NanoByte.StructureEditor.Sample.Model;
using NanoByte.StructureEditor.WinForms;

namespace NanoByte.StructureEditor.Sample.Controls;

/// <summary>
/// Structure editor for <see cref="AddressBook"/>s.
/// </summary>
public class AddressBookEditor : StructureEditor<AddressBook>
{
    public AddressBookEditor()
    {
        DescribeRoot("Address Book")
           .AddPlainList("Group", x => x.Groups);

        Describe<IContactContainer>()
           .AddPlainList("Contact", x => x.Contacts);

        Describe<Contact>()
           .AddProperty("Home Address", x => PropertyPointer.ForNullable(() => x.HomeAddress), new AddressEditor())
           .AddProperty("Work Address", x => PropertyPointer.ForNullable(() => x.WorkAddress), new AddressEditor())
           .AddList(x => x.PhoneNumbers)
           .AddElement("Landline Number", () => new LandlineNumber())
           .AddElement("Mobile Number", () => new MobileNumber());
    }

    public void Open()
    {
        using var dialog = new OpenFileDialog {Filter = "XML File (*.xml)|*.xml"};
        if (dialog.ShowDialog(this) == DialogResult.OK)
            Open(CommandManager<AddressBook>.Load(dialog.FileName));
    }

    public bool Save()
    {
        if (string.IsNullOrEmpty(CommandManager.Path))
            return SaveAs();
        else
        {
            CommandManager.Save(CommandManager.Path);
            return true;
        }
    }

    public bool SaveAs()
    {
        using var dialog = new SaveFileDialog {Filter = "XML File (*.xml)|*.xml"};
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            CommandManager.Save(dialog.FileName);
            return true;
        }
        else return false;
    }

    public bool Closing()
    {
        if (!CommandManager.UndoEnabled) return true;

        return Msg.YesNoCancel(this, "Save changes?", MsgSeverity.Warn) switch
        {
            DialogResult.Yes => Save(),
            DialogResult.No => true,
            _ => false
        };
    }
}

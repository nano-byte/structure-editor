// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
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
           .AddList("Group", x => x.Groups);

        Describe<IContactContainer>()
           .AddList("Contact", x => x.Contacts);

        Describe<Contact>()
           .AddRequiredProperty("Home Address", x => PropertyPointer.For(() => x.HomeAddress), new AddressEditor())
           .AddProperty("Work Address", x => PropertyPointer.ForNullable(() => x.WorkAddress), new AddressEditor())
           .AddPolymorphicList(x => x.PhoneNumbers)
           .AddElement("Landline Number", () => new LandlineNumber())
           .AddElement("Mobile Number", () => new MobileNumber());

        Describe<Contact>()
           .AddPolymorphicProperty(x => PropertyPointer.ForNullable(() => x.PrimaryNumber))
           .AddElement("Primary Landline Number", () => new LandlineNumber())
           .AddElement("Primary Mobile Number", () => new MobileNumber());
    }

    private const string FileFilter = "XML File (*.xml)|*.xml"
#if NET
                                    + "|JSON File (*.json)|*.json"
#endif
        ;

    private static IStructureSerializer GetSerializer(string path)
#if NET
        => Path.GetExtension(path).Equals(".json", StringComparison.OrdinalIgnoreCase)
            ? JsonStructureSerializer.Instance
            : XmlStructureSerializer.Instance;
#else
        => XmlStructureSerializer.Instance;
#endif

    public void Open()
    {
        using var dialog = new OpenFileDialog {Filter = FileFilter};
        if (dialog.ShowDialog(this) == DialogResult.OK)
            Open(SerializingCommandManager<AddressBook>.Load(dialog.FileName, GetSerializer(dialog.FileName)));
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
        using var dialog = new SaveFileDialog {Filter = FileFilter};
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            var serializer = GetSerializer(dialog.FileName);
            if (CommandManager is not SerializingCommandManager<AddressBook> {Serializer: var current} || current != serializer)
                Open(new SerializingCommandManager<AddressBook>(CommandManager.Target!, serializer));

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

// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.IO;
using Eto.Forms;
using NanoByte.Common;
using NanoByte.Common.EtoControls;
using NanoByte.StructureEditor.EtoForms;
using NanoByte.StructureEditor.Sample.Model;

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

    private static readonly FileFilter[] _fileFilters =
    [
        new("XML File", ".xml"),
        new("JSON File", ".json")
    ];

    private static IStructureSerializer GetSerializer(string path)
        => Path.GetExtension(path).Equals(".json", StringComparison.OrdinalIgnoreCase)
            ? JsonStructureSerializer.Instance
            : XmlStructureSerializer.Instance;

    public void Open()
    {
        using var dialog = new OpenFileDialog();
        foreach (var filter in _fileFilters) dialog.Filters.Add(filter);

        if (dialog.ShowDialog(this) == DialogResult.Ok)
            Open(SerializingCommandManager<AddressBook>.Load(dialog.FileName, GetSerializer(dialog.FileName)));
    }

    public bool Save()
    {
        // Apply input from controls that still have the focus
        CommitPendingChanges();

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
        CommitPendingChanges();

        using var dialog = new SaveFileDialog();
        foreach (var filter in _fileFilters) dialog.Filters.Add(filter);

        if (dialog.ShowDialog(this) == DialogResult.Ok)
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
        CommitPendingChanges();
        if (!CommandManager.UndoEnabled) return true;

        return Msg.YesNoCancel(this, "Save changes?", MsgSeverity.Warn) switch
        {
            DialogResult.Yes => Save(),
            DialogResult.No => true,
            _ => false
        };
    }
}

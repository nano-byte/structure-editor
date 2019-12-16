// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Windows.Forms;
using NanoByte.Common;
using NanoByte.Common.Controls;
using NanoByte.StructureEditor.Sample.Model;
using NanoByte.StructureEditor.WinForms;

namespace NanoByte.StructureEditor.Sample.Controls
{
    /// <summary>
    /// Editor for <see cref="Address"/> nodes in <see cref="AddressBook"/> structures.
    /// </summary>
    public class AddressEditor : NodeEditorBase<Address>
    {
        public AddressEditor()
        {
            var textBoxStreet = new HintTextBox {HintText = "Street", Dock = DockStyle.Top, Multiline = true, Height = 40};
            var textBoxCity = new HintTextBox {HintText = "City", Dock = DockStyle.Top};
            var textBoxCountry = new HintTextBox {HintText = "Country", Dock = DockStyle.Top};

            RegisterControl(textBoxStreet, PropertyPointer.For(() => Target!.Street));
            RegisterControl(textBoxCity, PropertyPointer.For(() => Target!.City));
            RegisterControl(textBoxCountry, PropertyPointer.For(() => Target!.Country));

            // Reverse order for "Dock"
            Controls.Add(textBoxCountry);
            Controls.Add(textBoxCity);
            Controls.Add(textBoxStreet);
        }
    }
}

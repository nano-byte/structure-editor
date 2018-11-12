// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;

namespace NanoByte.StructureEditor.Sample.Model
{
    public interface IContactContainer
    {
        List<Contact> Contacts { get; }
    }
}
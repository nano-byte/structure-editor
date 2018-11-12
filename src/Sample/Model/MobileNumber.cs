// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;

namespace NanoByte.StructureEditor.Sample.Model
{
    [Description("A phone number for a mobile phone.")]
    public class MobileNumber : PhoneNumber, IEquatable<MobileNumber>
    {
        public bool Equals(MobileNumber other)
            => base.Equals(other);
    }
}

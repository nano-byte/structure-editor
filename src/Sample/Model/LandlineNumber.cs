// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.ComponentModel;

namespace NanoByte.StructureEditor.Sample.Model
{
    [Description("A phone number for a landline.")]
    public class LandlineNumber : PhoneNumber, IEquatable<LandlineNumber>
    {
        public bool Equals(LandlineNumber other)
            => base.Equals(other);
    }
}

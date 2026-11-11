// Copyright Bastian Eicher
// Licensed under the MIT License

namespace NanoByte.StructureEditor.EtoForms;

/// <summary>
/// A control that may hold user input that has not been applied yet (e.g., because the control still has focus).
/// </summary>
public interface IPendingChanges
{
    /// <summary>
    /// Applies any user input that has not been applied yet.
    /// </summary>
    /// <remarks>Call this before relying on the data being edited to be up-to-date, e.g., before saving.</remarks>
    void CommitPendingChanges();
}

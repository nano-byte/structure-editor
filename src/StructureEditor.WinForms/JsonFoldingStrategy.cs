// Copyright Bastian Eicher
// Licensed under the MIT License

using System.Collections.Generic;
using ICSharpCode.TextEditor.Document;

namespace NanoByte.StructureEditor.WinForms;

/// <summary>
/// Folds JSON objects and arrays that span multiple lines.
/// </summary>
internal class JsonFoldingStrategy : IFoldingStrategy
{
    public List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation)
    {
        var markers = new List<FoldMarker>();
        var openings = new Stack<int>();
        string text = document.TextContent;
        bool inString = false;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (inString)
            {
                if (c == '\\') i++; // Skip escaped character
                else if (c == '"') inString = false;
                continue;
            }

            switch (c)
            {
                case '"':
                    inString = true;
                    break;
                case '{' or '[':
                    openings.Push(i);
                    break;
                case '}' or ']' when openings.Count > 0:
                    int start = openings.Pop();
                    if (document.GetLineNumberForOffset(start) != document.GetLineNumberForOffset(i))
                        markers.Add(new FoldMarker(document, start, i - start + 1, text[start] == '{' ? "{...}" : "[...]", isFolded: false));
                    break;
            }
        }

        return markers;
    }
}

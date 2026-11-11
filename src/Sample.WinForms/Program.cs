// Copyright Bastian Eicher
// Licensed under the MIT License

using System;
using System.Windows.Forms;

namespace NanoByte.StructureEditor.Sample;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
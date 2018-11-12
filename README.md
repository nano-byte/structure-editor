# NanoByte Structure Editor

[![NuGet](https://img.shields.io/nuget/v/NanoByte.StructureEditor.WinForms.svg)](https://www.nuget.org/packages/NanoByte.StructureEditor.WinForms/)
[![API documentation](https://img.shields.io/badge/api-docs-orange.svg)](https://structure-editor.nano-byte.net/)
[![Build status](https://img.shields.io/appveyor/ci/nano-byte/structure-editor.svg)](https://ci.appveyor.com/project/nano-byte/structure-editor)  
NanoByte Structure Editor is a WinForms library that helps you build split-screen editors for your data structures, consisting of:

1. a collapsible tree-view of the data structure,
2. a graphical editor for the currently selected element in the tree (PropertyGrid or custom) and
3. a text editor (based on [ICSharpCode.TextEditor](https://github.com/nano-byte/ICSharpCode.TextEditor)) with a serialized (XML, JSON, etc.) representation of the currently selected element.

This allows you to create an IDE-like experience for your users when editing complex domain specific languages, configuration files, etc..

This library is available for .NET Framework 2.0+.  
Take a look a the [sample project](src/Sample) for ideas how to use the library.

## Building

The source code is in [`src/`](src/), a project for API documentation is in [`doc/`](doc/) and generated build artifacts are placed in `artifacts/`.

You need [Visual Studio 2017](https://www.visualstudio.com/downloads/) to perform a full build of this project.  

Run `.\build.ps1`. This scripts takes a version number as an input argument. The source code itself contains no version numbers. Instead the version is picked by continuous integration using [GitVersion](http://gitversion.readthedocs.io/).

## Contributing

We welcome contributions to this project such as bug reports, recommendations and pull requests.

This repository contains an [EditorConfig](http://editorconfig.org/) file. Please make sure to use an editor that supports it to ensure consistent code style, file encoding, etc.. For full tooling support for all style and naming conventions consider using JetBrain's [ReSharper](https://www.jetbrains.com/resharper/) or [Rider](https://www.jetbrains.com/rider/) products.

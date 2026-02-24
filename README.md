# 🚀 BubaCode

**BubaCode** is a cross-platform code editor built from scratch for learning purposes using C# and Avalonia UI. This project demonstrates advanced text editing algorithms, efficient data structures, and modern UI patterns while providing a fully functional code editing experience.

![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-blue)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![Avalonia](https://img.shields.io/badge/Avalonia-11.3.9-orange)
> ⚠️ While Avalonia supports Windows, Linux, and macOS, this project has only been tested on Windows.

![gif](https://github.com/user-attachments/assets/e57bdc24-e015-444f-90f6-732ac1f44c40)


---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture Highlights](#architecture-highlights)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
- [Roadmap](#roadmap)

---

## Overview

BubaCode is a lightweight, cross-platform code editor designed as an educational project to explore:
- Advanced text editing algorithms (Piece Table implementation)
- Command pattern for undo/redo functionality
- Syntax highlighting and lexical analysis
- Cross-platform UI development with Avalonia
- MVVM architecture patterns

This project serves as a practical example of building complex desktop applications with modern C# and demonstrates real-world implementations of computer science concepts.

---

## Features

### Core Editing Features
- ✅ **Multi-line text editing** with full keyboard navigation
- ✅ **Text selection** with mouse and keyboard (Shift+Arrow keys)
- ✅ **Clipboard operations** (Copy: Ctrl+C, Paste: Ctrl+V)
- ✅ **Undo/Redo** with full state restoration (Ctrl+Z, Ctrl+Y)

### File Management
- 📁 **File Explorer** with tree view navigation
- 📂 **Folder operations**: Create, rename, delete folders
- 📄 **File operations**: Create, rename, delete, open files
- 💾 **Save functionality** with dirty state tracking (Ctrl+S)
- 🔄 **Multiple file tabs** with efficient caching
- 🗂️ **Drag and drop** support for file reordering

### Syntax Highlighting
- 🎨 **Syntax highlighting** for multiple languages:
  - C#, JavaScript, Python, CSS, Java, JSON
  - HTML with dedicated lexer
  - Plain text fallback
- 🔧 **Extensible lexer system** with factory pattern
- 🎯 **Token-based parsing** for accurate highlighting

### User Interface
- 🌓 **Modern Fluent design** using Avalonia UI
- 🎨 **Clean, minimalist interface**
- 👋 **Welcome screen** for quick project access
- ⚠️ **Error notifications** with user-friendly messages
- 💬 **Dialog system** for confirmations and input

### Advanced Features
- ⚡ **Piece Table data structure** for efficient text operations
- 🔄 **Command pattern** for all editing operations
- 📸 **Snapshot system** for perfect undo/redo state management
- ⌨️ **Extensible keyboard shortcuts** via registry system
- 🧩 **Command concatenation** for efficient typing history

---

## Architecture Highlights

### 1. **Piece Table Text Implementation**
BubaCode uses a sophisticated **Piece Table** data structure for text storage, providing:
- **O(1) insertion** and deletion at any position
- **Memory efficiency** by reusing original text buffer
- **Efficient undo/redo** through buffer snapshots
- **Fast line offset calculations** with separate line tracking

**Key Classes:**
- `PieceTableText`: Core piece table implementation with linked list of text pieces
- `PieceTableTextAdapter`: Adapter layer providing high-level text operations
- `TextSnapshot`: Immutable snapshots for undo/redo state restoration
- `TextLines`: Line offset tracking for efficient line-based operations

### 2. **Command Pattern Implementation**
All editing operations use the **Command Pattern**, enabling:
- **Consistent undo/redo** across all operations
- **Command history** management
- **Action composition** for complex operations
- **Separation of concerns** between UI and logic

**Text Editing Commands:**
- `TypeTextCommand`: Text insertion with concatenation support
- `RemoveCharacterCommand`: Backspace operations
- `RemoveFromSelectionCommand`: Delete selected text
- `EnterCommand`: New line with auto-indentation
- `PasteCommand`: Clipboard paste operations
- `CopyCommand`: Clipboard copy operations
- `UndoCommand` / `RedoCommand`: History navigation

### 3. **MVVM Architecture**
Clean separation using **Model-View-ViewModel**:
- **Models**: Business logic and data structures
- **ViewModels**: Presentation logic with CommunityToolkit.Mvvm
- **Views**: AXAML-based UI components
- **Services**: Cross-cutting concerns (files, clipboard, dialogs)

### 4. **Lexer System**
Extensible **syntax highlighting** architecture:
- `ILexer` interface for language-specific lexers
- `LexerFactory` for automatic lexer selection by file extension
- Token-based parsing for accurate syntax recognition
- Support for multiple language features (keywords, strings, comments)

---

## Technology Stack

- **Framework**: .NET 9.0
- **UI Framework**: Avalonia UI 11.3.9
- **MVVM Toolkit**: CommunityToolkit.Mvvm 8.2.2
- **Language**: C# with nullable reference types enabled
- **Architecture**: MVVM with Command Pattern
- **Platforms**: Windows, Linux, macOS

---


## Getting Started

### Prerequisites
- **.NET 9.0 SDK** or later
- **Visual Studio 2022** / **JetBrains Rider** / **VS Code** with C# extension

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/czareski/BubaCode.git
cd BubaCode
```

2. **Restore dependencies**
```bash
dotnet restore
```

3. **Build the project**
```bash
dotnet build
```

4. **Run the application**
```bash
dotnet run
```

### Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Ctrl+S` | Save |
| `Ctrl+Z` | Undo |
| `Ctrl+Y` | Redo |
| `Ctrl+C` | Copy selected text |
| `Ctrl+V` | Paste from clipboard |
| `Shift+←/→` | Select text |
| `Tab` | Insert tab / indent |
| `Enter` | New line with auto-indent |
| `Backspace` | Delete character |

---

## Roadmap

### Planned Features
- [ ] Theme customization
- [ ] More language support
- [ ] Find and Replace functionality
- [ ] code folding
- [ ] Plugin system
- [ ] Code completion and IntelliSense

### Known Limitations
- No native code completion
- Limited syntax highlighting (token-based, not semantic)
- Single non-resizable window instance
- too few commands for complex editing operations

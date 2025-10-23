# QuadX

Standalone Modding tool made to edit Resident Evil 4 (2005) data and map/level files within a 3D Editor.

[![Github license](https://img.shields.io/github/license/r3nzk/Re4QuadX.svg)](LICENSE) 
[![GitHub release](https://img.shields.io/github/release/r3nzk/Re4QuadX.svg)](https://github.com/r3nzk/Re4QuadX-Project//releases/latest)
[![Github All Releases](https://img.shields.io/github/downloads/r3nzk/Re4QuadX/total.svg)](https://github.com/r3nzk/Re4QuadX/releases/latest)

## About:
QuadX is a comprehensive 3D editor built to intuitively modify core files for most versions of OG RE4. It features a fully integrated 3D viewport for real-time manipulation.
This fork focuses on improving editing workflow, interfaces, automation, and overall editor stability.

<img width="1919" height="1056" alt="example1" src="https://github.com/user-attachments/assets/b459dc5a-12aa-4a01-8db5-0b05e7f45a45" />

## Fork Upgrades
* General interface/logic/structure rework with modern components and unified visual style.
* Renderer mipmap generation and new render loop for smoother 3D view.
* New GIZMO based object translation and rotation supporting local and worldspace.
* Built-in automation to extract, import and repack entire room files with 1 click.
* Console tab that displays editor actions with deeper setup logs and sanity checks.
* Search and Filter bars to multiple panels for easier element selection.
* Dark and Light theme support (Custom themes in the future).
* More options and preferences.

Check full changelog [here](https://github.com/r3nzk/RE4QuadX/commits/main).

## Usage
This editor is designed to be as intuitive and straightforward as possible. However, since it works with closed-source game, some of RE4’s file formats, structures, or engine quirks can be a bit unclear. If you need guidance, check the project [Wiki](https://github.com/r3nzk/Re4QuadX/wiki) for setup instructions and general usage tips.

To use the program properly, make sure you have the game files for your target version available. You can follow the step-by-step Setup Wizard that runs on first launch (or open it anytime from Misc > Setup Wizard). It will help you configure all required game directories and tool paths.

## Supported Versions
The application currently supports:
- Resident Evil 4 Ultimate HD Edition (UHD)
- Resident Evil 4 Sourcenext/Ubisoft (2007)
- Resident Evil 4 for Playstation 2
- Resident Evil 4 for Playstation 4/Nintendo Switch (Partialy)

## Libraries/Packages Utilized
* [JADERLINK_MODEL_VIEWER](https://github.com/JADERLINK/JADERLINK_MODEL_VIEWER)
* [TGASharpLib](https://github.com/ALEXGREENALEX/TGASharpLib)
* [DDSReaderSharp](https://github.com/ALEXGREENALEX/DDSReaderSharp)
* [ScarletLibrary](https://github.com/xdanieldzd/Scarlet)
* [Newtonsoft Json.NET](https://www.newtonsoft.com/json)
* [OpenTK](https://www.nuget.org/packages/OpenTK/)
* [OpenTK.GLControl](https://www.nuget.org/packages/OpenTK.GLControl)
* [PowerLib.Winform](https://www.nuget.org/packages/PowerLib.Winform)
* [RealTaiizor](https://github.com/Taiizor/ReaLTaiizor)

## Credits
QuadX is a fork of the [RE4QuadNewAge](https://github.com/JADERLINK/Re4QuadNewAge) Editor created by [JADERLINK](https://github.com/JADERLINK/).
Most of the heavy lifting, research, and reverse-engineering work were done by him and the RE4 modding community.
This fork mainly focuses on improving usability, adding quality-of-life features, and modernizing the interface, while keeping the original core and logic built entirely by jaderlink.

## License
MIT License – Free to use, modify, and distribute.

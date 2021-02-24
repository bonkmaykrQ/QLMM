# ![](bitmap0.png)

## [Get it on Gamebanana](https://gamebanana.com/tools/6969)
  
A mod manager for Quake Live.

Written and compiled in VS2019 using C# and WPF.

It works by scanning your game assets folder /baseq3/ for pk3 files and changing the file extension to disable/enable them, and also allowing you to view the metadata of each .pk3 by means of the pk3's package comments.

# System Requirements
- [.NET Framework 4.7.2 Runtime](https://dotnet.microsoft.com/download/dotnet-framework/net472)
- Windows 7 SP1 or newer

**Linux users: You will need Winetricks to run this application!**

# Installation
## Windows
Open `QLMM_setup.exe` and follow the on-screen directions. Remember where you install the program to, or create a shortcut on your desktop.

## Linux/Mac/BSD
If you don't already have WINE and Winetricks installed, install them from your package manager of choice.

Using Winetricks, select Select the default wineprefix > Install a Windows DLL or Component, and install `dotnet472` plus any other files you may need which you are missing.

Run wine `QLMM_setup.exe` in the folder where you saved the installer, and follow the on-screen directions. Remember where you install the program to, or create a shortcut on your desktop.

# Compatible Games
QLMM currently has official support for:
- Quake III: Arena
- Quake Live 

## Quake III: Arena
As of version 1.0.1.0, Quake III may show some extra files in the QLMM window that you did not install. This does not happen with Quake Live since Quake Live's default filenames are blacklisted from the file search. You can customize the blacklist in future versions to mitigate this problem, but until then, you should compile your own copy of QLMM if you want a custom blacklist.

## Quake Live
Quake Live has out-of-box support, and if is installed to the right location QLMM will set everything up for you.

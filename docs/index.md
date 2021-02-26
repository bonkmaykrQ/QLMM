#### [← Back to Homepage](https://bonkmaykrq.github.io/)
# ![](bitmap0.png)

## [Get it on Gamebanana](https://gamebanana.com/tools/6969)
  
A mod manager for Quake Live.

Written and compiled in VS2019 using C# and WPF.

It works by scanning your game assets folder /baseq3/ for pk3 files and changing the file extension to disable/enable them, and also allowing you to view the metadata of each .pk3 by means of the pk3's package comments.

# System Requirements
- [.NET Framework 4.7.2 Runtime](https://dotnet.microsoft.com/download/dotnet-framework/net472)
- Windows 7 SP1 or newer

**Linux users: You will need Winetricks to run this application!**

# Building
Building requires Visual Studio 2019 or any similar version. There is no GNU/Linux version of QLMM or VS2019, so if you are using WINE, you will need to use a pre-compiled binary instead.  
If you haven't already, install Visual Studio 2019, and any neccessary dependencies.  
    
Open the .sln file with Visual Studio, and press F7.  
  
Note that you may need to disable assembly signing in the project settings, and recompile the program dependencies without signing too, if the build fails due to signing or strong-name related problems.

# Installation
## Windows
Open `QLMM_setup.exe` and follow the on-screen directions. Remember where you install the program to, or create a shortcut on your desktop.

## Linux/Mac/BSD
If you don't already have WINE and Winetricks installed, install them from your package manager of choice.

Using Winetricks, select Select the default wineprefix > Install a Windows DLL or Component, and install `dotnet472` plus any other files you may need which you are missing.

Run `wine QLMM_setup.exe` in the folder where you saved the installer, and follow the on-screen directions. Remember where you install the program to, or create a shortcut on your desktop.

# Compatible Games
QLMM currently has official support for:
- Quake III: Arena
- Quake Live
- Warsow (or Warfork) 

## Quake III: Arena
As of version 1.0.1.0, Quake III may show some extra files in the QLMM window that you did not install. This does not happen with Quake Live since Quake Live's default filenames are blacklisted from the file search. You can customize the blacklist in future versions to mitigate this problem, but until then, you should compile your own copy of QLMM if you want a custom blacklist.

## Quake Live
Quake Live has out-of-box support, and if is installed to the right location QLMM will set everything up for you.

## Warsow
Warsow uses a custom version of the Quake II engine called QFusion, but it uses the same file formats as idTech3, so it is compatible with QLMM. However, the same issue with Quake III: Arena is here: a custom blacklist will be needed in order to hide the stock game files.  
  
Keep in mind that Warsow organizes itself a little differently. In the game's install folder, the baseq3 folder is usually found at Warfork.app\Contents\Resources\basewf\ as opposed to simply baseq3\.

# Importing Mods
idTech3 uses the ZIP format, often also labeled as PK3, to store game assets.  
  
When importing, QLMM will ask you for which mod file you wish to import.  
If you were to look for said mod in the baseq3 folder, you may notice that the file name is completely different from the file you just imported. This is for two reasons:  
 - Quake Live's main assets are stored in pak00.pk3. Quake Live loads pk3s in a specific order, so naming your mod starting with pak01_whateverblahblah helps prevent the mods from being cancelled out by the game itself.
 - Some mods may have the same file name, so to prevent QLMM from overwriting one you already have, it will add a random number to the end of the file name, followed by the extension.  

# Creating Metadata
QLMM reads metadata from the mod's comments, not a file inside. This allows us to store our own data inside of the PK3, without interfereing with the game, and without having to extract the mod (incase it is incredibly large).  
  
To create metadata, you will need a ZIP archiving tool (e.g., WinRAR), with the ability to edit comments.  
  
Paste this template into your PK3's comments, after removing any existing comments. Edit the values as needed:  
```
{
  "qlmm": {
    "name": "Put your mod's name here.",
    "author": "Put the creator's name here.",
    "description": "Put your description here.",
    "version": "1"
  }
}
```

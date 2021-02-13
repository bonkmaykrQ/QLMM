# Quake Live Mod Manager
Free software mod manager for Quake Live, designed for Microsoft Windows 10.

# Building
Building requires Visual Studio 2019 or any similar version. There is no GNU/Linux version of QLMM or VS2019, so if you are using WINE, you will need to use a pre-compiled binary instead.  
If you haven't already, install Visual Studio 2019, and any neccessary dependencies.  
    
Open the .sln file with Visual Studio, and press F7.  
  
Note that you may need to disable assembly signing in the project settings, and recompile the program dependencies without signing too, if the build fails due to signing or strong-name related problems. **Official builds from me will contain my certificate, so that you know that the version you are downloading was compiled and written by me, and has no malware in it.**

# Usage
## Importing Mods
idTech3 uses the ZIP format, often also labeled as PK3, to store game assets.  
  
When importing, QLMM will ask you for which mod file you wish to import.  
If you were to look for said mod in the baseq3 folder, you may notice that the file name is completely different from the file you just imported. This is for two reasons:  
 - Quake Live's main assets are stored in pak00.pk3. Quake Live loads pk3s in a specific order, so naming your mod starting with pak01_whateverblahblah helps prevent the mods from being cancelled out by the game itself.
 - Some mods may have the same file name, so to prevent QLMM from overwriting one you already have, it will add a random number to the end of the file name, followed by the extension.  

## Creating Metadata
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

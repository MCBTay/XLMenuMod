# XLMenuMod
The mod allows you to use subfolders within your Maps and Gear directories for SkaterXL!

As of the V3 beta, the Maps folder did not allow you to place maps in subdirectories at all.  The Gear folder allowed you to place gear in subdirectories, but wouldn't render it that way in game.

## Features
### Maps

<img align="left" src="maps.gif">

* This mod allows you to place your custom maps in any subdirectory structure you want (within Windows path limits), and will render that directory structure in the game's UI.
  * Any map that needs an additional DLL file with it for extensions, the mod expects the DLL to be in the same folder with the same name as the map file.
  * Compatible with (but somewhat replaces the need for) DawgVinci's Map Extension Mod.  Any extension DLLs only get loaded up if they aren't already loaded by another mod.
* Sorting
  * The following sort methods are supported: Name ascending, name descending, filesize ascending, filesize descending, newest, and oldest.
    * For filesize sorting, a folder will be treated as the sum of it's contents.
    * For newest/oldest sorting, a folder will be treated as it's oldest/newest content.




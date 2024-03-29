# XLMenuMod
The mod allows you to customize your menu and use subfolders within your Maps and Gear directories for SkaterXL!

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=MCBTay_XLMenuMod&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=MCBTay_XLMenuMod)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=MCBTay_XLMenuMod&metric=bugs)](https://sonarcloud.io/summary/new_code?id=MCBTay_XLMenuMod)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=MCBTay_XLMenuMod&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=MCBTay_XLMenuMod)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=MCBTay_XLMenuMod&metric=coverage)](https://sonarcloud.io/summary/new_code?id=MCBTay_XLMenuMod)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=MCBTay_XLMenuMod&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=MCBTay_XLMenuMod)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=MCBTay_XLMenuMod&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=MCBTay_XLMenuMod)

[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=MCBTay_XLMenuMod&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=MCBTay_XLMenuMod)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=MCBTay_XLMenuMod&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=MCBTay_XLMenuMod)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=MCBTay_XLMenuMod&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=MCBTay_XLMenuMod)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=MCBTay_XLMenuMod&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=MCBTay_XLMenuMod)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=MCBTay_XLMenuMod&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=MCBTay_XLMenuMod)

## Features
### Maps
* This mod allows you to place your custom maps in any subdirectory structure you want (within Windows path limits), and will render that directory structure in the game's UI.
  * Any map with a preview image will also display in folders as long as the preview image is beside the map.
  * Any map that needs an additional DLL file with it for extensions, the mod expects the DLL to be in the same folder with the same name as the map file.  Any extension DLLs only get loaded up if they aren't already loaded by another mod.
* **Sorting**
  * The following sort methods are supported: Name ascending, name descending, filesize ascending, filesize descending, newest, oldest, author ascending, and author descending.
    * For filesize sorting, a folder will be treated as the sum of it's contents.
    * For newest/oldest sorting, a folder will be treated as it's oldest/newest content.
    * For author sorting, any level (including folders) without an author will be ignored.
    
### Gear
* This mod groups the built in official gear into brand folders.
* This mod groups hair by style, and then you pick color.
* This mod allows you to place your custom gear in any subdirectory structure you want (within Windows path limits), and will render that directory structure in the game's UI.
* **Sorting**
  * The following sort methods are supported: Name ascending, name descending, newest, and oldest.
    * For newest/oldest sorting, a folder will be treated as it's oldest/newest content.

## Options
* Pressing the B/O button to back out of a folder once you've selected it is enabled by default.  You can disable it in the mod settings.
* There's a setting to shrink down the default font size of the Gear/Maps lists.  By default, the font size is 36 which is a little large.  Currently you can set it to Normal (36), Small (30), Smaller (24). 
  * Note: Because of the way the game recycles list objects, it's recommended to set this value prior to entering the gear/maps menus OR to restart the game after setting this value.  This does not work for all menus.
* Dark Mode - Will turn all of the menus to a dark background with light colored text.
* Hide Official Gear option will hide all official gear except for skin tones and hair.

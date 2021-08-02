# streamdeck-starcitizen

**WORK IN PROGRESS**

TODO :

* Test multiple language keyboards (only lightly tested 3.13 with US keyboard)

* Automatically update binding in streamdeck plugin when changing binding in Star Citizen options screen (now streamdeck.exe has to be restarted)

* Add more logging to pluginlog.log

* Allow switching between LIVE and PTU versions. (Currently gets binding only from LIVE installation)

**Elgato Stream Deck button plugin for Star Citizen**

![Elgato Stream Deck button plugin for Star Citizen](https://i.imgur.com/SgsPN2d.png)

This plugin connects to Star Citizen to get the key bindings.

**The pluging does not contain any button images or ready made streamdeck profiles.**

Credit goes to https://github.com/SCToolsfactory/SCJMapper-V2 for all the code to get the defaultProfile.xml from the pk4 file etc.

The button works in a similar way, to the streamdeck 'Hotkey' button type.
So, there is only one image and there is no game state feedback for these buttons.
The differences with the 'Hotkey' buttons are, that it gets the keyboard binding from the game.
When the stream deck button is pushed, the 'key down' event is sent to the keyboard
and only after the stream deck button is released, the 'key up' event is sent to the keyboard.

A sound can be played when pressing a button.

**You can clear the sound path, by clicking on the label in front of the file picker edit box.**

The buttons can also be used with multi-action buttons.

After you install the plugin in the streamdeck software, then there will be a new button type in the streamdeck software.

Choose a button in the streamdeck software (drag and drop), then choose a Star Citizen function for that button (that must have a keyboard binding in Star Citizen!) and then choose any picture for that button.

Add an image to a button in this way:

![Button Image](https://i.imgur.com/xkgy7uZ.png)

Animated gif files are supported.

When the plugin is first started, it finds and opens the game file :

`C:\Program Files\Roberts Space Industries\StarCitizen\LIVE\Data.p4k`

and extracts `defaultProfile.xml` and also english text resources. This could take more than 10 seconds.

Compressed versions (files ending in .scj) are cached in the plugin directory and should be automatically refreshed, the next time Star Citizen is updated to a new version.

You can also delete the .scj files and restart the plugin, to extract the files from the p4k file again.

For easier debugging, installation and testing, `defaultProfile.xml`, `dropdowntemplate.html` and `keybindings.csv` files are created in the plugin directory.

The plugin uses all the active keyboard bindings from `defaultProfile.xml` and then overrules some of the bindings, with any custom keyboard bindings from this file :

`C:\Program Files\Roberts Space Industries\StarCitizen\LIVE\USER\Client\0\Profiles\default\actionmaps.xml`

The `dropdowntemplate.html` can be used to reconfigure the `%appdata%\Elgato\StreamDeck\Plugins\com.mhwlng.starcitizen.sdPlugin\PropertyInspector\StarCitizen\Static.html` file.

This may be needed, in case more custom keyboard bindings were added to `actionmaps.xml`, that didn't have any corresponding keyboard bindings in `defaultProfile.xml`.

The plugin installer is here: https://github.com/mhwlng/streamdeck-starcitizen/releases

To install the plugin, double click the file `com.mhwlng.starcitizen.streamDeckPlugin` which should install the plugin.

(This only works, if the plugin not already installed. Otherwise you will need to uninstall or remove the plugin first.)

This .streamDeckPlugin file is a zip file and the contents are simply copied to :

`%appdata%\Elgato\StreamDeck\Plugins\com.mhwlng.starcitizen.sdPlugin`

To update to a new version :

Stop the Stream Deck application:

`c:\Program Files\Elgato\StreamDeck\StreamDeck.exe`

Then delete the `%appdata%\Elgato\StreamDeck\Plugins\com.mhwlng.starcitizen.sdPlugin` directory. (make a backup copy first)

Then start the streamdeck software again.

Then double click the file `com.mhwlng.starcitizen.streamDeckPlugin` as usual.

MAKE SURE that you save any images, profiles etc. that you put in these directories yourself, BEFORE deleting the directory.
And put them back after the installation.
The plugin installer doesn't come with button images.

The button configurations are not stored in the plugin directory.

After uninstalling and re-installing the plugin, all the button definition should still be there.

The com.mhwlng.starcitizen.sdPlugin directory contains a pluginlog.log file, which may be useful for troubleshooting.

Thanks to :

https://github.com/BarRaider/streamdeck-tools

https://github.com/SCToolsfactory/SCJMapper-V2

https://github.com/ishaaniMittal/inputsimulator

https://nerdordie.com/product/stream-deck-key-icons/


# commonItems.NET
Common items for the different game converters. Including parsers, OS utilities, and some common classes.
## Current Status
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/f3e8a38e2925476390f79399a70b4985)](https://www.codacy.com/gh/ParadoxGameConverters/commonItems.NET/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=ParadoxGameConverters/commonItems.NET&amp;utm_campaign=Badge_Grade)
[![CodeFactor](https://www.codefactor.io/repository/github/paradoxgameconverters/commonitems.net/badge)](https://www.codefactor.io/repository/github/paradoxgameconverters/commonitems.net)
[![Coverage Status](https://coveralls.io/repos/github/ParadoxGameConverters/commonItems.NET/badge.svg?branch=main)](https://coveralls.io/github/ParadoxGameConverters/commonItems.NET?branch=main)

## Included Items

### Color.cs
Represents a Paradox-defined color.

Can be directly created in either the RGB or HSV color spaces.

Can be imported in:  
*   Unspecified with ints (becomes RGB) - "= { 64 128 128 }"
*   Unspecified with floats (becomes RGB) - "= { 0.5 0.9 0.1 }"
*   RGB - "= rgb { 64 128 128 }"
*   Hex - "= hex { 408080 }"
*   HSV - "= hsv { 0.5 0.5 0.5 }"
*   HSV360 - "= hsv360 { 180 50 50 }"
*   Name (requires caching definitions for the named colors in advance) - "= dark_moderate_cyan"

Can be output in:  
*   unspecified (rgb) - "= { 64 128 128 }"
*   RGB - "= rgb { 64 128 128 }"
*   hex - "= hex { 408080 }"
*   HSV - "= hsv { 0.5 0.5 0.5 }"
*   HSV360 - "= hsv360 { 180 50 50 }"

The individual components can be accessed in both RGB and HSV color spaces, equality and inequality can be checked, the color cache can be reviewed and modified, and colors can have a random fluctuation be applied automatically.

### CommonFunctions.cs
A handful of helpful commonly-used functions.

##### TrimPath
Given a file with path included (such as '/this/is/a/path.txt' or 'c:\this\is\a\path.txt'), returns the part that's just the filename ('path.txt')

##### TrimExtension
Given a filename with an extension (such as 'file.extension' or 'file.name.with.extension'), returns the extension ('extension')

##### ReplaceCharacter
Given a string (such as 'a file name.eu4'), replaces all instances of the specified character (such as ' ') with underscores (resulting in 'a_file_name.eu4')

##### CardinalToOrdinal
Given a cardinal number (1, 2, 15), returns the equivalent ordinal word ending ('st', 'nd', 'th') for appending to the numbers ('1st', '2nd', '15th')

##### CardinalToRoman
Given a number (3, 12, 2020), returns the number in roman numerals ('III', 'XII', 'MMXX')

##### NormalizeStringPath
Given a path, normalizes it in a standard way for all converters that all supported Paradox games will recognize (by replacing all spaces, dashes, and other weird characters (<, >, :, ?...) with underscores, and by converting entire string into ASCII)

### Date.cs
A class representing a Paradox-style date.

##### Construction
*   Default construction gives a date of 0001-01-01
*   Can directly specify year, month, day
*   Can directly specify year, month, day, and if this is an AUC (years after the founding of Rome, used in Imperator) format date or not
*   Can pass a paradox-style string specifying the date
*   Can pass a paradox-style string specifying the date, and if this is an AUC (years after the founding of Rome, used in Imperator) format date or not
 
##### Comparison
Dates can be compared using all the standard comparators. Additionally, the difference between two dates (in years) can be found.

##### Modification
Dates can be increased by months or years, and can be decreased by years. In all cases thse must be whole-number changes to months or years.

##### Output
Dates can be output to a stream or converted to a string.

### GameVersion.cs
A class and some helpers representing the version of a Paradox game. Assumes the version consists of four integers (1.12.4.5), but versions with fewer parts will work seamlessly.

#### GameVersion
The version class itself.

##### Construction
*   Default construction gives a version of 0.0.0.0
*   Can directly specify all four parts
*   Can construct via string - "1.2.3.4", "1.6.7", ""
*   Can construct via stream - "version = { first = 1 second = 2 third = 3 forth = 4 }"
    *   The misspelling of 'fourth' is Paradox's
   
##### Comparison
GameVersions can be compared using all the standard comparators. It is a simple lexicographic comparison in order of the parts.

#### Output
A freestanding output function allows writing a GameVersion to output streams.

### Logger.cs
A class to log information during conversion. Everything is logged to log.txt in the directory used to run the program. No configuration or setup is required.

There are dedicated methods for each level, but the level can also be passed as argument in the .Log method:
```Logger.Info($"Message: {variable}");```
```Logger.Log(LogLevel.Info, $"Message: {variable}");```

Log level specifies a message at the beginning of the logged line, and can be set to any of the following:  
*   LogLevel.Debug
*   LogLevel.Info
*   LogLevel.Warn
*   LogLevel.Error
*   LogLevel.Notice
*   LogLevelProgress - this is used to set the progress bar in the frontend by passing an integer specifying the percentage. Unnecessary in programs without frontend.

#### GetAllFilesInFolder
Returns the filenames of all files in the specified folder.

#### GetAllSubfolders
Returns the filenames of all subfolders in the specified folder.

#### GetAllFilesInFolderRecursive
Returns the filenames of all files in the specified folder and all its subfolders.

#### GetCurrentDirectoryWString
Returns the current directory in UTF-16.

#### getSteamInstallPath
Given a Steam AppId, returns the install path for the corresponding game.

#### TryCreateFolder
Attempts to create the specified directory.

#### TryCopyFile
Attempts to copy the specified file to the specified location, overwriting any existing file.

#### CopyFolder
Attempts to recursively copy to specified folder to the specified location.

#### RenameFolder
Attempts to rename the specified folder to the specified name.

#### WriteToConsole
Writes the specified message to the console at the specified log level. On Windows this colors the message appropriately.

#### DeleteFolder
Attempt to delete the specified folder (and everything in it).

### Parser.cs
Description coming soon.

### ParserHelpers.cs
Description coming soon.

### StringUtils.cs
Description coming soon.

# commonItems.NET
Common items for the different game converters. Including parsers, OS utilities, and some common classes.

## Current Status
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/f3e8a38e2925476390f79399a70b4985)](https://www.codacy.com/gh/ParadoxGameConverters/commonItems.NET/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=ParadoxGameConverters/commonItems.NET&amp;utm_campaign=Badge_Grade)
[![CodeFactor](https://www.codefactor.io/repository/github/paradoxgameconverters/pgcg.commonitems/badge)](https://www.codefactor.io/repository/github/paradoxgameconverters/pgcg.commonitems)
[![Coverage Status](https://coveralls.io/repos/github/ParadoxGameConverters/commonItems.NET/badge.svg?branch=main)](https://coveralls.io/github/ParadoxGameConverters/commonItems.NET?branch=main)

## Included Items

### Parsing
The core of the library: a parser for Paradox-format text ("keyword = value" pairs, brace-delimited blocks, comments starting with #, quoted strings, @variables, and @[...] interpolated expressions).

#### BufferedReader.cs
A wrapper around a StreamReader that allows pushing characters back into the stream, with typed getters for reading Paradox-format values.

##### Construction
*   Default - empty reader
*   From a string
*   From a Stream
*   From a StreamReader

##### Reading
*   Read() / Read(uint) - read a single character or a chunk of characters
*   ReadLine(), ReadToEnd(), Peek(), EndOfStream
*   Skip(uint) - skip a number of characters
*   PushBack(char) - return a character to the stream for later reads

##### Typed Getters
*   GetString(), GetChar(), GetInt(), GetLong(), GetULong(), GetDouble(), GetFloat()
*   GetStrings(), GetInts(), GetLongs(), GetULongs(), GetDoubles(), GetFloats() - read lists of values
*   GetStringOfItem() - read a raw item as a StringOfItem
*   GetBool() - parses "yes"/"no"
*   GetAssignments() - reads a list of "key = value" pairs
*   GetDouble() / GetFloat() overloads accepting a ScriptValueCollection for resolving script values
*   Variables: the Variables dictionary stores @variables encountered during parsing, ResolveVariable() resolves them, EvaluateExpression() evaluates @[...] expressions, and CopyVariables() copies variables from another reader

#### Parser.cs
Parses Paradox-format streams and files, dispatching tokens to registered handlers.

*   RegisterKeyword(keyword, handler) - register an exact keyword (O(1) lookup)
*   RegisterRegex(regex, handler) - register a regular expression rule
*   Handlers come in two shapes: `Del(reader, keyword)` and `SimpleDel(reader)`
*   ParseStream(reader), ParseFile(path), ParseFolder(...), ParseGameFolder(...), and ParseGameFile(...) - the game-folder variants read through a ModFilesystem
*   Quoted tokens are automatically retried without quotes, so handlers registered for unquoted keywords still match quoted input
*   Construction with `implicitVariableHandling: true` automatically captures `@var = value` assignments into the reader's Variables
*   AbsorbBOM(reader) - skips a leading byte-order mark
*   GetNextLexeme(reader) / GetNextTokenWithoutMatching(reader) - static helpers for manual token extraction
*   ClearRegisteredRules() - removes all registered rules

#### ParserBuilder.cs
A fluent builder for creating parsers:
```csharp
var parser = new ParserBuilder()
	.WithKeyword("name", reader => name = reader.GetString())
	.WithRegex(CommonRegexes.Catchall, ParserHelpers.IgnoreItem)
	.IgnoreAndLogUnregisteredItems()
	.Build();
```
*   WithKeyword, WithRegex, IgnoreUnregisteredItems, IgnoreAndLogUnregisteredItems, IgnoreAndStoreUnregisteredItems
*   The builder constructor also accepts implicitVariableHandling

#### ParserExtensions.cs
Convenience extensions for parsers:
*   IgnoreUnregisteredItems() - silently skip anything unregistered
*   IgnoreAndLogUnregisteredItems() - skip and log unregistered items
*   IgnoreAndStoreUnregisteredItems(set) - skip and collect unregistered tokens into the given set

#### ParserHelpers.cs
Helper functions for handling items during parsing:
*   IgnoreItem(reader) - skip a single item, handling the "=" or "?=" separator, the "rgb"/"hsv" prefixes, and braced blocks while respecting quotes, comments, and interpolated expressions

#### CommonRegexes.cs
Precompiled regular expressions for common token shapes, usable with RegisterRegex:
*   Catchall, String, QuotedString
*   Integer, QuotedInteger, Float, QuotedFloat
*   Date
*   Variable (@name), InterpolatedExpression (@[...])

#### StringOfItem.cs
Captures a raw item from a stream as a string, preserving the original text exactly:
*   "value", "= value", or "= { ... }" including nested braces and quoted content
*   IsArrayOrObject() - reports whether the item is a brace-delimited block

#### BlobList.cs
Reads a braced list and splits it into the individual top-level blocks ("blobs"), each preserved as a raw string.

#### StringUtils.cs
Small helpers for working with quoted strings:
*   IsQuoted() - checks whether a string is wrapped in quotes
*   RemQuotes() - strips surrounding quotes
*   AddQuotes() - wraps a string in quotes

### Colors

#### Color.cs
Represents a Paradox-defined color.

Can be directly created in either the RGB or HSV color spaces, with optional alpha.

Can be imported in:  
*   Unspecified with ints (becomes RGB) - "= { 64 128 128 }"
*   Unspecified with floats (becomes RGB) - "= { 0.5 0.9 0.1 }"
*   Unspecified with four floats (becomes RGBA) - "= { 0.5 0.9 0.1 0.1 }"
*   RGB - "= rgb { 64 128 128 }"
*   Hex - "= hex { 408080 }"
*   HSV - "= hsv { 0.5 0.5 0.5 }"
*   HSVA - "= hsv { 0.5 0.5 0.5 0.1 }"
*   HSV360 - "= hsv360 { 180 50 50 }"
*   Name (requires the named colors to be loaded in advance via ColorFactory) - "= dark_moderate_cyan"

Can be output in:  
*   unspecified (rgb) - "= { 64 128 128 }"
*   RGB - "= rgb { 64 128 128 }"
*   hex - "= hex { 408080 }"
*   HSV - "= hsv { 0.5 0.5 0.5 }"
*   HSV360 - "= hsv360 { 180 50 50 }"

The individual components can be accessed in both RGB (R, G, B) and HSV (H, S, V) color spaces, as well as alpha (A). Equality and inequality can be checked, colors can be constructed from a System.Drawing.Color, and RGB components are clamped to 0-255 on construction.

#### ColorFactory.cs
Parses a color from a BufferedReader, handling all the input formats listed above (prefixed rgb/hex/hsv/hsv360, unprefixed int and float lists, and RGBA float lists). Named colors can be cached in the factory's NamedColors dictionary for later lookup by name.

#### NamedColorCollection.cs
A sorted dictionary of named colors, populated from game files:
*   LoadNamedColors(relativePath, modFilesystem) - parses "colors = { name = color ... }" blocks from the given folder in the mod filesystem

### Dates and Versions

#### Date.cs
A struct representing a Paradox-style date.

##### Construction
*   Default construction gives a date of 0001-01-01
*   Can directly specify year, month, day
*   Can directly specify year, month, day, and if this is an AUC (years after the founding of Rome, used in Imperator) format date or not
*   Can pass a paradox-style string specifying the date
*   Can pass a paradox-style string specifying the date, and if this is an AUC format date or not
*   Can construct from a DateTimeOffset
*   Strings convert implicitly to Date
 
##### Comparison
Dates can be compared using all the standard comparators. Additionally, the difference between two dates (in years) can be found with DiffInYears.

##### Modification
Dates can be increased or decreased by days, months, or years (ChangeByDays, ChangeByMonths, ChangeByYears). In all cases these must be whole-number changes.

##### Output
Dates can be converted to a string or a DateTimeOffset, and can be serialized with PDXSerializer.

#### GameVersion.cs
A class and some helpers representing the version of a Paradox game. Assumes the version consists of four integers (1.12.4.5), but versions with fewer parts will work seamlessly, and any part can be a wildcard.

##### Construction
*   Default construction gives a version of 0.0.0.0
*   Can directly specify all four parts (each optional)
*   Can construct via string - "1.2.3.4", "1.6.7", "1.2.*", ""
*   Can construct via stream - "version = { first = 1 second = 2 third = 3 forth = 4 }"
    *   The misspelling of 'fourth' is Paradox's
   
##### Comparison
GameVersions can be compared using all the standard comparators. It is a simple lexicographic comparison in order of the parts.

##### Conversion and Helpers
*   ToString() - full four-part form, missing parts become 0
*   ToShortString() - only the parts that were specified
*   ToWildCard() - missing parts become "*"
*   IsLargerishThan(other) - fuzzy "compatible up to X" comparison
*   IsModCompatibleWithGame(modVersion, gameVersion) - wildcard-aware compatibility check used for mods
*   ExtractVersionFromLauncher(filePath) - extracts the installation version from a launcher-settings.json

#### ConverterVersion.cs
Loads and describes a converter's version file:
*   LoadVersion(file) / LoadVersion(reader) - parses "name", "version", "source", "target", "minSource", "maxSource", "minTarget", and "maxTarget"
*   GetDescription() - e.g. "Compatible with [game] [v1.2-v1.5] and [game] [v1.2-v1.5]"
*   ToString() - the standard converter banner

### Script Values and Defines

#### Defines.cs
Reads and stores defines from common/defines files in Jomini games:
*   LoadDefines(modFS) - parses all defines files through the mod filesystem
*   GetValue(category, key) - returns a define's value as a string

#### ScriptValueCollection.cs
Loads script values from common/script_values and exposes them as a read-only dictionary of string to double:
*   LoadScriptValues(modFS, defines) - reads all script value files, resolving references between values, define: references, @variables, and @[...] expressions. The files are re-read until no new values are added, so forward references work.

### Logging

#### Logger.cs
A static facade over log4net used for all logging during conversion. Must be configured before use:
*   Configure() - console logging
*   Configure(logToConsole, logToFile) - optionally also logs to log.txt (rolling, 100 MB max, 5 backups)
*   Configure(appenders) - use custom log4net appenders

There are dedicated methods for each level, plus Format variants for each:
```csharp
Logger.Info($"Message: {variable}");
Logger.InfoFormat("Message: {0}", variable);
Logger.Log(Level.Info, "Message");
```

Log level specifies a message at the beginning of the logged line, and can be set to any of the following:  
*   Debug
*   Info
*   Warn
*   Error
*   Notice
*   Progress - this is used to set the progress bar in the frontend by passing an integer specifying the percentage (Logger.Progress(50) or Logger.IncrementProgress()). Unnecessary in programs without a frontend.

#### LogExtensions.cs
Extensions backing the logging facade:
*   ProgressLevel - the custom log4net "PROGRESS" level
*   Progress(this ILog, int) / IncrementProgress(this ILog, limit) - report progress as a percentage
*   Notice(this ILog, string) / NoticeFormat - log at notice level
*   Log(this ILog, Level, string) - log at an arbitrary level

### System Utilities

#### CommonFunctions.cs
A handful of helpful commonly-used functions.

##### Path Functions
*   SplitPath - splits a path on '/' and '\\' into its components
*   TrimPath - given a file with path included (such as '/this/is/a/path.txt' or 'c:\this\is\a\path.txt'), returns the part that's just the filename ('path.txt')
*   GetPath - the inverse of TrimPath; returns everything up to and including the last path separator
*   TrimExtension - given a filename with an extension (such as 'file.extension' or 'file.name.with.extension'), returns the filename without the extension
*   GetExtension - given a filename with an extension, returns just the extension

##### String Functions
*   ReplaceCharacter - given a string (such as 'a file name.eu4'), replaces all instances of the specified character (such as ' ') with underscores (resulting in 'a_file_name.eu4')
*   ToOrdinalSuffix - given a cardinal number (1, 2, 15), returns the equivalent ordinal word ending ('st', 'nd', 'th') for appending to the numbers ('1st', '2nd', '15th'). Supports several languages (english, french, spanish, catalan, dutch, german, italian, japanese, portuguese, russian, chinese, ...)
*   ToRomanNumeral - given a number (3, 12, 2020), returns the number in roman numerals ('III', 'XII', 'MMXX')
*   LanguageNameToIetfTag - converts a Paradox language name ('english', 'simp_chinese') into an IETF language tag ('en', 'zh')
*   NormalizeStringPath - given a path, normalizes it in a standard way for all converters that all supported Paradox games will recognize (by replacing all spaces, dashes, and other weird characters (<, >, :, ?...) with underscores, and by converting the entire string into ASCII)
*   NormalizeUTF8Path - the UTF-8 to ASCII portion of NormalizeStringPath (invalid path characters become underscores)

##### Game Installation Paths
*   GetSteamInstallPath(steamAppId) - given a Steam AppId, returns the install path for the corresponding game, or null
*   GetGOGInstallPath(gogId) - given a GOG game ID (see gogdb.org), returns the install path for the corresponding game, or null. On Linux, Wine prefixes are searched.

#### SystemUtils.cs
Filesystem helpers:
*   GetAllFilesInFolder, GetAllSubfolders, GetAllFilesInFolderRecursive - sorted lists of relative paths
*   TryCreateFolder, TryCopyFile, TryCopyFolder, TryRenameFolder, TryDeleteFolder - safe filesystem operations that log errors and return bool instead of throwing

#### DebugInfo.cs
Logs diagnostic information about the environment:
*   LogSystemInfo() - operating system and installed UI language
*   LogCPUInfo() - CPU names
*   LogExecutableDirectory() - directory of the running executable
*   LogEverything() - all of the above

#### EncodingConversions.cs
*   ConvertUTF8ToASCII() - converts a UTF-8 string to ASCII, dropping non-representable characters

#### ASCIIStringExtensions.cs
*   FoldToASCII() - transliterates non-ASCII characters to their ASCII equivalents where possible (e.g. 'à' becomes 'a')

### Collections

#### OrderedSet.cs
A set that preserves insertion order (implements ISet<T>).

#### IdObjectCollection.cs
A collection of objects keyed by their Id (objects must implement IIdentifiable<TKey>). Supports Add, TryAdd, AddOrReplace, Remove, RemoveAll, and lookup by key.

#### ConcurrentIdObjectCollection.cs
A thread-safe version of IdObjectCollection, backed by a ConcurrentDictionary.

#### IIdentifiable.cs
Interfaces for objects with an Id: IIdentifiable and IIdentifiable<TKey>. IIdentifiable<TKey> provides comparison by Id and a GetIdString() used by PDXSerializer.

#### ExtensionMethods.cs
*   ToOrderedSet() - converts an enumerable into an OrderedSet
*   RemoveWhere() - removes all items matching a predicate from a collection

### Mods

#### Mod.cs
Represents a game mod: name, path, supported game version, dependencies, and replaced folders.

#### ModParser.cs
A parser for .mod files:
*   ParseMod(file) / ParseMod(reader) - reads "name", "path"/"archive", "dependencies", "replace_path", and "supported_version"
*   IsValid() - whether name and path were both found
*   IsCompressed() - whether the mod is an archive (zip/bin)

#### ModLoader.cs
Loads and verifies the mods referenced by a savegame:
*   LoadMods(gameDocumentsPath, incomingMods, gameVersion, throwForOutOfDateMods) - verifies mod files, unpacks compressed mods into the converter's folder, checks game-version compatibility (throwing UserErrorException for incompatible mods if requested), and fills UsableMods

#### ModFilesystem.cs
A virtual filesystem layering the game's root folder with the loaded mods:
*   Handles file precedence (mods override the base game, later mods override earlier ones) and "replaced folders" that hide base-game content
*   GetActualFileLocation(path) / GetActualFolderLocation(path) - resolve a path to its real location on disk
*   GetAllFilesInFolder / GetAllSubfolders / GetAllFilesInFolderRecursive - enumerate contents across the overlay, returning ModFSFileInfo items

#### ModFSFileInfo.cs
A struct describing a file found through the mod filesystem: FromMod, RelativePath, and AbsolutePath.

### Localization

#### LocBlock.cs
The localization for a single key across languages:
*   Indexer by language, with fallback to the base language
*   HasLocForLanguage(language)
*   ModifyForEveryLanguage(...) - applies a delegate to every language's loc (e.g. replacing $ADJ$ placeholders)
*   GetYmlLocLineForLanguage(language) - renders the entry as a yml line

#### LocDB.cs
A collection of LocBlocks (IdObjectCollection<string, LocBlock>):
*   ScrapeLocalizations(modFS) - reads all *.yml files from the "localization" folder in parallel, merging entries across languages and files

#### LocDelegate.cs
Delegates for modifying localizations: LocDelegate(baseLoc, languageName) and TwoArgLocDelegate(baseLoc, modifyingLoc, languageName).

### Linguistics

#### CharacterExtensions.cs
*   IsVowel() / IsConsonant() - classification of characters, including accented letters

#### StringExtensions.cs
Helpers for generating adjectives from country/state names:
*   TrimNonAlphanumericEnding(), TrimNonLetterEnding()
*   GetAdjective() - derives the adjectival form using the rules in adjective_rules.txt and adjective_rewrite_rules.txt (consonant/vowel endings, rewrite iterations, ASCII folding)

### Serialization

#### PDXSerializer.cs
Serializes .NET objects into Paradox-format text:
*   Serialize(obj, indent, withBraces) - supports IPDXSerializable objects, strings, dictionaries, enumerables, IIdentifiable collections, KeyValuePairs, bools ("yes"/"no"), and value types

#### IPDXSerializable.cs
Interface for types with custom serialization: Serialize(indent, withBraces).

#### SerializedName.cs
Attribute for overriding the property name used during serialization.

#### SerializeOnlyValue.cs
Attribute marking a property to be serialized as its value only.

#### NonSerialized.cs
Attribute marking a property to be skipped during serialization.

### Exceptions

#### ConverterException.cs
The base exception type for converter errors.

#### UserErrorException.cs
An exception for errors that should be shown to the user (e.g. incompatible mod versions), deriving from ConverterException.

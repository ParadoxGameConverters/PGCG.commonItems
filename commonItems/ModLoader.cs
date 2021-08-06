using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

using Name = System.String;
using Path = System.String;
using Mods = System.Collections.Generic.List<Mod>;

public class Mod {
    public Name name = "";
    public Path path = "";
    public SortedSet<Name> dependencies = new();
    public Mod() { }
    public Mod(Name name, Path path) {
        this.name = name;
        this.path = path;
    }
    public Mod(Name name, Path path, SortedSet<Name> dependencies) {
        this.name = name;
        this.path = path;
        this.dependencies = dependencies;
    }

    public override bool Equals(object? obj) {
        return obj is Mod mod &&
               name == mod.name &&
               System.IO.Path.GetFullPath(path) == System.IO.Path.GetFullPath(mod.path);
    }
    public override int GetHashCode() {
        return HashCode.Combine(name, path);
    }
}


namespace commonItems {
    public class ModLoader {
        private Mods possibleUncompressedMods = new(); // name, absolute path to mod directory
        private Mods possibleCompressedMods = new(); // name, absolute path to zip file
        public Mods UsableMods { get; private set; } = new(); // name, absolute path for directories, relative for unpacked
        
        public ModLoader() { }

        public void LoadMods (string gameDocumentsPath, Mods incomingMods) {
            if (incomingMods.Count == 0) {
                // We shouldn't even be here if the save didn't have mods! Why were Mods called?
                Logger.Log(LogLevel.Info, "No mods were detected in savegame. Skipping mod processing.");
                return;
            }

            // We enter this function with a List of (optional) mod names and (required) mod file locations from the savegame.
            // We need to read all the mod files, check their paths (and potential archives for ancient mods) unpack what's
            // necessary, and exit with a vector of updated mod names (savegame can differ from actual mod file) and mod folder
            // locations.

            // The function below reads all the incoming .mod files and verifies their internal paths/archives are correct and
            // point to something present on disk. No unpacking yet.
            LoadModDirectory(gameDocumentsPath, incomingMods);

            // Now we merge all detected .mod files together.
            Logger.Log(LogLevel.Info, "\tDetermining mod usability");
            var allMods = new Mods();
            allMods.AddRange(possibleUncompressedMods);
            allMods.AddRange(possibleCompressedMods);

            // With a list of all detected and matched mods, we unpack the compressed ones (if any) and store the results.
            foreach(var mod in allMods) {
                // This invocation will unpack any compressed mods into our converter's folder, and skip already unpacked ones.
                var possibleModPath = UncompressAndReturnNewPath(mod.name);
                if (possibleModPath == null) {
                    Logger.Log(LogLevel.Warning, "\t\tFailure unpacking " + mod.name + ", skipping this mod at your risk.");
                    continue;
                }

                // All verified mods go into usableMods
                Logger.Log(LogLevel.Info, "\t\t->> Found potentially useful [" + mod.name + "]: " + possibleModPath + "/");
                UsableMods.Add(new Mod(mod.name, possibleModPath, mod.dependencies));
            }
        }
        private void LoadModDirectory(string gameDocumentsPath, Mods incomingMods) {
            var modsPath = System.IO.Path.Combine(gameDocumentsPath, "mod");
            if (!Directory.Exists(modsPath)) {
                throw new DirectoryNotFoundException("Mods directory path is invalid! Is it at: " + modsPath + " ?");
            }

            Logger.Log(LogLevel.Info, "\tMods directory is " + modsPath);

            var diskModNames = SystemUtils.GetAllFilesInFolder(modsPath);
            foreach (var mod in incomingMods) {
                var trimmedModFileName = CommonFunctions.TrimPath(mod.path);

                if (!diskModNames.Contains(trimmedModFileName)) {
                    if (string.IsNullOrEmpty(mod.name)) {
                        Logger.Log(LogLevel.Warning, "\t\tSavegame uses mod at " + mod.path +
                            ", which is not present on disk. Skipping at your risk, but this can greatly affect conversion.");
                    } else {
                        Logger.Log(LogLevel.Warning, "\t\tSavegame uses [" + mod.name + "] at " + mod.path +
                            ", which is not present on disk. Skipping at your risk, but this can greatly affect conversion.");
                    }
                    continue;
                }

                if (CommonFunctions.GetExtension(trimmedModFileName) != "mod") {
                    continue; // shouldn't be necessary but just in case
                }

                // Attempt parsing the .mod file
                var theMod = new ModParser();
                var modFilePath = System.IO.Path.Combine(modsPath, trimmedModFileName);
                try {
                    theMod.ParseMod(modFilePath);
                } catch(Exception e) {
                    Logger.Log(LogLevel.Warning, "\t\tError while reading " + modFilePath +
                        "! Mod will not be useable for conversions.");
                    continue;
                }
                ProcessLoadedMod(theMod, mod.name, trimmedModFileName, mod.path, modsPath, gameDocumentsPath);
            }
        }

        void ProcessLoadedMod(ModParser theMod, string modName, string modFileName, string modPath, string modsPath, string gameDocumentsPath) {
            var modFilePath = System.IO.Path.Combine(modsPath, modFileName);
            if (!theMod.IsValid()) {
                Logger.Log(LogLevel.Warning, "\t\tMod at " + modFilePath + " does not look valid.");
                return;
            }

            // Fix potential pathing issues.
            var modPathAtDocuments = System.IO.Path.Combine(gameDocumentsPath, theMod.Path);
            if (!theMod.IsCompressed() && !Directory.Exists(theMod.Path)) {
                // Maybe we have a relative path
                if (Directory.Exists(modPathAtDocuments)) {
                    // fix this.
                    theMod.Path = modPathAtDocuments;
                } else {
                    WarnForInvalidPath(theMod, modName, modPath);
                    return;
                }
            } else if (theMod.IsCompressed() && !File.Exists(theMod.Path)) {
                // Maybe we have a relative path
                if (File.Exists(modPathAtDocuments)) {
                    // fix this.
                    theMod.Path = modPathAtDocuments;
                } else {
                    WarnForInvalidPath(theMod, modName, modPath);
                    return;
                }
            }

            // file under category.
            FileUnderCategory(theMod, modFilePath);
        }

        void WarnForInvalidPath(ModParser theMod, string name, string path) {
            if (string.IsNullOrEmpty(name)) {
                Logger.Log(LogLevel.Warning, "\t\tMod at " + path + " points to " + theMod.Path +
                      " which does not exist! Skipping at your risk, but this can greatly affect conversion.");
            } else {
                Logger.Log(LogLevel.Warning, "\t\tMod [" + name + "] at " + path + " points to " + theMod.Path + " which does not exist! Skipping at your risk, but this can greatly affect conversion.");
            }
        }

        void FileUnderCategory(ModParser theMod, string path) {
            if (!theMod.IsCompressed()) {
                possibleUncompressedMods.Add(new Mod(theMod.Name, theMod.Path, theMod.Dependencies));
                Logger.Log(LogLevel.Info, "\t\tFound a potential mod [" + theMod.Name + "] with a mod file at " + path
                                          + " and itself at " + theMod.Path);
            } else {
                possibleCompressedMods.Add(new Mod(theMod.Name, theMod.Path, theMod.Dependencies));
                Logger.Log(LogLevel.Info, "\t\tFound a compressed mod [" + theMod.Name + "] with a mod file at " + path
                                          + " and itself at " + theMod.Path);
            }
        }

        string? UncompressAndReturnNewPath(string modName) {
            foreach (var mod in possibleUncompressedMods) {
                if (mod.name == modName) {
                    return mod.path;
                }
            }

            foreach (var compressedMod in possibleCompressedMods) {
                if (compressedMod.name != modName) {
                    continue;
                }
                string uncompressedName = System.IO.Path.GetFileNameWithoutExtension(compressedMod.path);

                SystemUtils.TryCreateFolder("mods");

                var uncompressedPath = System.IO.Path.Combine("mods", uncompressedName);
                if (!Directory.Exists(uncompressedPath)) {
                    Logger.Log(LogLevel.Info, "\t\tUncompressing: " + compressedMod.path);
                    if (!ExtractZip(compressedMod.path, uncompressedPath)) {
                        Logger.Log(LogLevel.Warning, "We're having trouble automatically uncompressing your mod.");
                        Logger.Log(LogLevel.Warning, "Please, manually uncompress: " + compressedMod.path);
                        Logger.Log(LogLevel.Warning, "Into converter's folder, mods/" + uncompressedName + " subfolder.");
                        Logger.Log(LogLevel.Warning, "Then run the converter again. Thank you and good luck.");
                        return null;
                    }
                }

                if (Directory.Exists(uncompressedPath)) {
                    return uncompressedPath;
                }
                return null;
            }

            return null;
        }

        bool ExtractZip(string archive, string path) {
            SystemUtils.TryCreateFolder(path);
            try {
                using (var modFile = new ZipFile(archive)) {
                    var enumerator = modFile.GetEnumerator();
                    while (enumerator.MoveNext()) {
                        var entry = (ZipEntry)enumerator.Current;
                        var inPath = entry.Name;
                        var name = System.IO.Path.GetFileName(inPath);
                        if (entry.IsDirectory) {
                            continue;
                        }

                        // Does target directory exist?
                        var dirNamePos = inPath.IndexOf(name);

                        var dirName = System.IO.Path.Combine(path, inPath.Substring(0, dirNamePos));
                        if (!Directory.Exists(dirName)) {
                            // We need to craft our way through to target directory.
                            var remainder = inPath;
                            var currentPath = path;
                            while (remainder != name) {
                                var pos = remainder.IndexOf('/');
                                if (pos != -1) {
                                    var makeDirName = remainder.Substring(0, pos);
                                    currentPath = System.IO.Path.Combine(currentPath, makeDirName);
                                    SystemUtils.TryCreateFolder(currentPath);
                                    remainder = remainder.Substring(pos + 1);
                                } else {
                                    break;
                                }
                            }
                        }
                        new FastZip().ExtractZip(archive, System.IO.Path.Combine(path, inPath), inPath);
                    }
                }
            } catch(Exception e) {
                Logger.Log(LogLevel.Error, "Extracting zip failed: " + e);
                return false;
            }

            return true;
        }
    }
}

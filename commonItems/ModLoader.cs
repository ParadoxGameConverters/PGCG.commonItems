using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using Mods = System.Collections.Generic.List<commonItems.Mod>;

namespace commonItems {
	public class ModLoader {
		private readonly Mods possibleUncompressedMods = new(); // name, absolute path to mod directory
		private readonly Mods possibleCompressedMods = new(); // name, absolute path to zip file
		public Mods UsableMods { get; } = new(); // name, absolute path for directories, relative for unpacked

		public void LoadMods(string gameDocumentsPath, Mods incomingMods) {
			if (incomingMods.Count == 0) {
				// We shouldn't even be here if the save didn't have mods! Why were Mods called?
				Logger.Info("No mods were detected in savegame. Skipping mod processing.");
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
			Logger.Info("\tDetermining mod usability");
			var allMods = new Mods();
			allMods.AddRange(possibleUncompressedMods);
			allMods.AddRange(possibleCompressedMods);

			// With a list of all detected and matched mods, we unpack the compressed ones (if any) and store the results.
			foreach (var mod in allMods) {
				// This invocation will unpack any compressed mods into our converter's folder, and skip already unpacked ones.
				var possibleModPath = UncompressAndReturnNewPath(mod.Name);
				if (possibleModPath == null) {
					Logger.Warn("\t\tFailure unpacking " + mod.Name + ", skipping this mod at your risk.");
					continue;
				}

				// All verified mods go into usableMods
				Logger.Info("\t\t->> Found potentially useful [" + mod.Name + "]: " + possibleModPath + "/");
				UsableMods.Add(new Mod(mod.Name, possibleModPath, mod.Dependencies));
			}
		}
		private void LoadModDirectory(string gameDocumentsPath, Mods incomingMods) {
			var modsPath = System.IO.Path.Combine(gameDocumentsPath, "mod");
			if (!Directory.Exists(modsPath)) {
				throw new DirectoryNotFoundException("Mods directory path is invalid! Is it at: " + modsPath + " ?");
			}

			Logger.Info("\tMods directory is " + modsPath);

			var diskModNames = SystemUtils.GetAllFilesInFolder(modsPath);
			foreach (var mod in incomingMods) {
				var trimmedModFileName = CommonFunctions.TrimPath(mod.Path);

				if (!diskModNames.Contains(trimmedModFileName)) {
					if (string.IsNullOrEmpty(mod.Name)) {
						Logger.Warn("\t\tSavegame uses mod at " + mod.Path +
							", which is not present on disk. Skipping at your risk, but this can greatly affect conversion.");
					} else {
						Logger.Warn("\t\tSavegame uses [" + mod.Name + "] at " + mod.Path +
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
				} catch (Exception e) {
					Logger.Warn("\t\tError while reading " + modFilePath +
						"! Mod will not be useable for conversions. Exception: " + e);
					continue;
				}
				ProcessLoadedMod(theMod, mod.Name, trimmedModFileName, mod.Path, modsPath, gameDocumentsPath);
			}
		}

		private void ProcessLoadedMod(ModParser theMod, string modName, string modFileName, string modPath, string modsPath, string gameDocumentsPath) {
			var modFilePath = System.IO.Path.Combine(modsPath, modFileName);
			if (!theMod.IsValid()) {
				Logger.Warn("\t\tMod at " + modFilePath + " does not look valid.");
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

		private static void WarnForInvalidPath(ModParser theMod, string name, string path) {
			if (string.IsNullOrEmpty(name)) {
				Logger.Warn("\t\tMod at " + path + " points to " + theMod.Path +
					  " which does not exist! Skipping at your risk, but this can greatly affect conversion.");
			} else {
				Logger.Warn("\t\tMod [" + name + "] at " + path + " points to " + theMod.Path + " which does not exist! Skipping at your risk, but this can greatly affect conversion.");
			}
		}

		private void FileUnderCategory(ModParser theMod, string path) {
			if (!theMod.IsCompressed()) {
				possibleUncompressedMods.Add(new Mod(theMod.Name, theMod.Path, theMod.Dependencies));
				Logger.Info("\t\tFound a potential mod [" + theMod.Name + "] with a mod file at " + path
										  + " and itself at " + theMod.Path);
			} else {
				possibleCompressedMods.Add(new Mod(theMod.Name, theMod.Path, theMod.Dependencies));
				Logger.Info("\t\tFound a compressed mod [" + theMod.Name + "] with a mod file at " + path
										  + " and itself at " + theMod.Path);
			}
		}

		private string? UncompressAndReturnNewPath(string modName) {
			foreach (var mod in possibleUncompressedMods) {
				if (mod.Name == modName) {
					return mod.Path;
				}
			}

			foreach (var compressedMod in possibleCompressedMods) {
				if (compressedMod.Name != modName) {
					continue;
				}
				string uncompressedName = System.IO.Path.GetFileNameWithoutExtension(compressedMod.Path);

				SystemUtils.TryCreateFolder("mods");

				var uncompressedPath = System.IO.Path.Combine("mods", uncompressedName);
				if (!Directory.Exists(uncompressedPath)) {
					Logger.Info("\t\tUncompressing: " + compressedMod.Path);
					if (!ExtractZip(compressedMod.Path, uncompressedPath)) {
						Logger.Warn("We're having trouble automatically uncompressing your mod.");
						Logger.Warn("Please, manually uncompress: " + compressedMod.Path);
						Logger.Warn("Into converter's folder, mods/" + uncompressedName + " subfolder.");
						Logger.Warn("Then run the converter again. Thank you and good luck.");
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

		private static bool ExtractZip(string archive, string path) {
			try {
				new FastZip().ExtractZip(archive, path, ".*");
			} catch (Exception e) {
				Logger.Error("Extracting zip failed: " + e);
				return false;
			}

			return true;
		}
	}
}

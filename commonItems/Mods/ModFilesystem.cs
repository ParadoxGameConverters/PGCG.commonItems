using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace commonItems.Mods;

public class ModFilesystem {
	private static bool PathIsReplaced(string path, IEnumerable<string> replacedPaths) {
		Logger.Debug($"Checking if path \"{path}\" is replaced...");
		
		var splitPath = CommonFunctions.SplitPath(path);

		return replacedPaths.Any(replacedPath => {
			var splitReplacedPath = CommonFunctions.SplitPath(replacedPath);
			for (int i = 0; i < splitReplacedPath.Length; ++i) {
				if (i >= splitPath.Length) {
					Logger.Info("NOT replaced: reached end of splitPath");
					return false;
				}

				if (splitPath[i] != splitReplacedPath[i]) {
					Logger.Info($"NOT replaced: {splitPath[i]} != {splitReplacedPath[i]}");
					return false;
				}
			}

			Logger.Info("REPLACED");
			return true;
		});
	}

	private readonly string gameRoot;
	private readonly List<Mod> mods;

	/// <summary>
	/// The constructor establishes the root of the filesystem.
	/// 
	/// </summary>
	/// <param name="gameRoot">Points at the game's root folder, and all paths in the lookup functions will be based on that root.</param>
	/// <param name="mods">A list of the mods applied, in increasing order of precedence. Later mods will override files in the game root or earlier mods, and their
	/// replace_paths will block earlier mods and the game It is the caller's responsibility to sort the mods appropriately</param>
	public ModFilesystem(string gameRoot, IEnumerable<Mod> mods) {
		this.gameRoot = gameRoot;
		this.mods = mods.ToList();
	}

	// lookup functions
	//[[nodiscard]] std::optional<std::string> GetActualFileLocation(const std::string& path) const;
	//[[nodiscard]] std::optional<std::string> GetActualFolderLocation(const std::string& path) const;
	//[[nodiscard]] std::set<std::string> GetAllFilesInFolder(const std::string& path) const;
	//[[nodiscard]] std::set<std::string> GetAllSubfolders(const std::string& path) const;
	//[[nodiscard]] std::set<std::string> GetAllFilesInFolderRecursive(const std::string& path) const;

	public string? GetActualFileLocation(string path) {
		foreach (var mod in Enumerable.Reverse(mods)) {
			var pathInMod = Path.Combine(mod.Path, path).Replace('\\', '/');
			if (File.Exists(pathInMod)) {
				return pathInMod;
			}
			if (PathIsReplaced(path, mod.ReplacedFolders)) {
				return null;
			}
		}

		// check game root
		var pathInGameRoot = Path.Combine(gameRoot, path).Replace('\\', '/');
		return File.Exists(pathInGameRoot) ? pathInGameRoot : null;
	}
	
	public string? GetActualFolderLocation(string path) {
		foreach (var mod in Enumerable.Reverse(mods)) {
			var pathInMod = Path.Combine(mod.Path, path).Replace('\\', '/');
			if (Directory.Exists(pathInMod)) {
				return pathInMod;
			}
			if (PathIsReplaced(path, mod.ReplacedFolders)) {
				return null;
			}
		}

		// check game root
		var pathInGameRoot = Path.Combine(gameRoot, path).Replace('\\', '/');
		return Directory.Exists(pathInGameRoot) ? pathInGameRoot : null;
	}

	public ISet<string> GetAllFilesInFolder(string path) {
		var fullFiles = new HashSet<string>();
		var foundFiles = new HashSet<string>();

		foreach (var mod in Enumerable.Reverse(mods)) {
			var pathInMod = Path.Combine(mod.Path, path).Replace('\\', '/');
			foreach (var newFile in SystemUtils.GetAllFilesInFolder(pathInMod)) {
				if (foundFiles.Contains(newFile)) {
					continue;
				}

				foundFiles.Add(newFile);
				fullFiles.Add( Path.Combine(pathInMod, newFile).Replace('\\', '/'));
			}

			if (PathIsReplaced(path, mod.ReplacedFolders)) {
				return fullFiles;
			}
		}

		var pathInGameRoot = Path.Combine(gameRoot, path).Replace('\\', '/');
		foreach (var newFile in SystemUtils.GetAllFilesInFolder(pathInGameRoot)) {
			if (foundFiles.Contains(newFile)) {
				continue;
			}

			foundFiles.Add(newFile);
			fullFiles.Add(Path.Combine(pathInGameRoot, newFile).Replace('\\', '/'));
		}

		return fullFiles;
	}
	
	public ISet<string> GetAllSubfolders(string path) {
		var fullFolders = new HashSet<string>();
		var foundFolders = new HashSet<string>();

		foreach (var mod in Enumerable.Reverse(mods)) {
			var pathInMod = Path.Combine(mod.Path, path).Replace('\\', '/');
			foreach (var newFolder in SystemUtils.GetAllSubfolders(pathInMod)) {
				if (foundFolders.Contains(newFolder)) {
					continue;
				}

				foundFolders.Add(newFolder);
				fullFolders.Add(Path.Combine(pathInMod, newFolder).Replace('\\', '/'));
			}

			if (PathIsReplaced(path, mod.ReplacedFolders)) {
				return fullFolders;
			}
		}

		var pathInGameRoot = Path.Combine(gameRoot, path).Replace('\\', '/');
		foreach (var newFolder in SystemUtils.GetAllSubfolders(pathInGameRoot)) {
			if (foundFolders.Contains(newFolder)) {
				continue;
			}

			foundFolders.Add(newFolder);
			fullFolders.Add(Path.Combine(pathInGameRoot, newFolder).Replace('\\', '/'));
		}

		return fullFolders;
	}
	
	public ISet<string> GetAllFilesInFolderRecursive(string path) {
		var fullFiles = new HashSet<string>();
		var foundFiles = new HashSet<string>();

		foreach (var mod in Enumerable.Reverse(mods)) {
			var pathInMod = Path.Combine(mod.Path, path).Replace('\\', '/');
			foreach (var newFile in SystemUtils.GetAllFilesInFolderRecursive(pathInMod)) {
				if (foundFiles.Contains(newFile)) {
					continue;
				}

				foundFiles.Add(newFile);
				fullFiles.Add(Path.Combine(pathInMod, newFile).Replace('\\', '/'));
			}

			if (PathIsReplaced(path, mod.ReplacedFolders)) {
				return fullFiles;
			}
		}

		var pathInGameRoot = Path.Combine(gameRoot, path).Replace('\\', '/');
		foreach (var newFile in SystemUtils.GetAllFilesInFolderRecursive(pathInGameRoot)) {
			if (foundFiles.Contains(newFile)) {
				continue;
			}

			foundFiles.Add(newFile);
			fullFiles.Add(Path.Combine(pathInGameRoot, newFile).Replace('\\', '/'));
		}

		return fullFiles;
	} 
}
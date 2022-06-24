using commonItems.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace commonItems.Mods;

public class ModFilesystem {
	private static bool PathIsReplaced(string path, IEnumerable<string> replacedPaths) {
		var splitPath = CommonFunctions.SplitPath(path);

		return replacedPaths.Any(replacedPath => {
			var splitReplacedPath = CommonFunctions.SplitPath(replacedPath);
			for (int i = 0; i < splitReplacedPath.Length; ++i) {
				if (i >= splitPath.Length) {
					return false;
				}

				if (splitPath[i] != splitReplacedPath[i]) {
					return false;
				}
			}

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
	/// replace_paths will block earlier mods and the game.
	/// It is the caller's responsibility to sort the mods appropriately
	/// </param>
	public ModFilesystem(string gameRoot, IEnumerable<Mod> mods) {
		this.gameRoot = gameRoot;
		this.mods = mods.ToList();
	}

	#region lookup functions
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

	public OrderedSet<string> GetAllFilesInFolder(string path) {
		var foundFiles = new SortedDictionary<string, string>(); // <relative path, full path>

		foreach (var mod in Enumerable.Reverse(mods)) {
			var pathInMod = Path.Combine(mod.Path, path).Replace('\\', '/');
			foreach (var newFile in SystemUtils.GetAllFilesInFolder(pathInMod)) {
				if (foundFiles.ContainsKey(newFile)) {
					continue;
				}

				var fullPath = Path.Combine(pathInMod, newFile).Replace('\\', '/');
				foundFiles.Add(newFile, fullPath);
			}

			if (PathIsReplaced(path, mod.ReplacedFolders)) {
				return new OrderedSet<string>(foundFiles.Values);
			}
		}

		var pathInGameRoot = Path.Combine(gameRoot, path).Replace('\\', '/');
		foreach (var newFile in SystemUtils.GetAllFilesInFolder(pathInGameRoot)) {
			if (foundFiles.ContainsKey(newFile)) {
				continue;
			}

			var fullPath = Path.Combine(pathInGameRoot, newFile).Replace('\\', '/');
			foundFiles.Add(newFile, fullPath);
		}

		return new OrderedSet<string>(foundFiles.Values);
	}
	
	public OrderedSet<string> GetAllSubfolders(string path) {
		var foundFolders = new SortedDictionary<string, string>(); // <relative path, full path>

		foreach (var mod in Enumerable.Reverse(mods)) {
			var pathInMod = Path.Combine(mod.Path, path).Replace('\\', '/');
			foreach (var newFolder in SystemUtils.GetAllSubfolders(pathInMod)) {
				if (foundFolders.ContainsKey(newFolder)) {
					continue;
				}

				var fullPath = Path.Combine(pathInMod, newFolder).Replace('\\', '/');
				foundFolders.Add(newFolder, fullPath);
			}

			if (PathIsReplaced(path, mod.ReplacedFolders)) {
				return new OrderedSet<string>(foundFolders.Values);
			}
		}

		var pathInGameRoot = Path.Combine(gameRoot, path).Replace('\\', '/');
		foreach (var newFolder in SystemUtils.GetAllSubfolders(pathInGameRoot)) {
			if (foundFolders.ContainsKey(newFolder)) {
				continue;
			}

			var fullPath = Path.Combine(pathInGameRoot, newFolder).Replace('\\', '/');
			foundFolders.Add(newFolder, fullPath);
		}

		return new OrderedSet<string>(foundFolders.Values);
	}
	
	public OrderedSet<string> GetAllFilesInFolderRecursive(string path) {
		var fullFiles = new OrderedSet<string>();
		var foundFiles = new OrderedSet<string>();

		var pathInGameRoot = Path.Combine(gameRoot, path).Replace('\\', '/');
		foreach (var newFile in SystemUtils.GetAllFilesInFolderRecursive(pathInGameRoot)) {
			if (foundFiles.Contains(newFile)) {
				continue;
			}

			foundFiles.Add(newFile);
			fullFiles.Add(Path.Combine(pathInGameRoot, newFile).Replace('\\', '/'));
		}

		foreach (var mod in mods) {
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

		return fullFiles;
	}
	#endregion
}
using commonItems.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace commonItems.Mods;

public sealed class ModFilesystem {
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
	
	private sealed class DefaultFilePrecedenceComparer : IComparer<string> {
		public int Compare(string? x, string? y) {
			if (x is null && y is null) {
				return 0;
			}
			if (x is null) {
				return -1;
			}
			if (y is null) {
				return 1;
			}

			var xSlashCount = x.Count(c => c is '/' or '\\');
			var ySlashCount = y.Count(c => c is '/' or '\\');
			if (xSlashCount < ySlashCount) {
				return -1;
			}
			if (xSlashCount > ySlashCount) {
				return 1;
			}
			return string.Compare(x, y, StringComparison.Ordinal);
		}
	}
	private static readonly DefaultFilePrecedenceComparer precedenceComparer = new();

	private readonly string gameRoot;
	private readonly Mod[] mods;

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
		this.mods = mods.ToArray();
	}

	#region lookup functions
	public string? GetActualFileLocation(string path) {
		foreach (var mod in mods.Reverse()) {
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
		foreach (var mod in mods.Reverse()) {
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

	public List<ModFSFileInfo> GetAllFilesInFolder(string path, IComparer<string> filePrecedenceComparer) {
		var foundFiles = new SortedDictionary<string, ModFSFileInfo>(filePrecedenceComparer); // <relative path, full path>

		foreach (var mod in mods.Reverse()) {
			var pathInMod = Path.Combine(mod.Path, path).Replace('\\', '/');
			foreach (var newFile in SystemUtils.GetAllFilesInFolder(pathInMod)) {
				if (foundFiles.ContainsKey(newFile)) {
					continue;
				}

				var fullPath = Path.Combine(pathInMod, newFile).Replace('\\', '/');
				foundFiles.Add(newFile, new(
					fromMod: true, newFile.Replace('\\', '/'), fullPath)
				);
			}

			if (PathIsReplaced(path, mod.ReplacedFolders)) {
				return foundFiles.Values.ToList();
			}
		}

		var pathInGameRoot = Path.Combine(gameRoot, path).Replace('\\', '/');
		foreach (var newFile in SystemUtils.GetAllFilesInFolder(pathInGameRoot)) {
			if (foundFiles.ContainsKey(newFile)) {
				continue;
			}

			var fullPath = Path.Combine(pathInGameRoot, newFile).Replace('\\', '/');
			foundFiles.Add(newFile, new(
				fromMod: false, newFile.Replace('\\', '/'), fullPath)
			);
		}

		return foundFiles.Values.ToList();
	}
	public List<ModFSFileInfo> GetAllFilesInFolder(string path) {
		return GetAllFilesInFolder(path, precedenceComparer);
	}
	
	public OrderedSet<string> GetAllSubfolders(string path, IComparer<string> folderPrecedenceComparer) {
		var foundFolders = new SortedDictionary<string, string>(folderPrecedenceComparer); // <relative path, full path>

		foreach (var mod in mods.Reverse()) {
			var pathInMod = Path.Combine(mod.Path, path).Replace('\\', '/');
			foreach (var newFolder in SystemUtils.GetAllSubfolders(pathInMod)) {
				if (foundFolders.ContainsKey(newFolder)) {
					continue;
				}

				var fullPath = Path.Combine(pathInMod, newFolder).Replace('\\', '/');
				foundFolders.Add(newFolder, fullPath);
			}

			if (PathIsReplaced(path, mod.ReplacedFolders)) {
				return foundFolders.Values.ToOrderedSet();
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

		return foundFolders.Values.ToOrderedSet();
	}

	public OrderedSet<string> GetAllSubfolders(string path) {
		return GetAllSubfolders(path, precedenceComparer);
	}

	public List<ModFSFileInfo> GetAllFilesInFolderRecursive(string path, IComparer<string> filePrecedenceComparer) {
		var foundFiles = new SortedDictionary<string, ModFSFileInfo>(filePrecedenceComparer); // <relative path, full path>

		foreach (var mod in mods.Reverse()) {
			var pathInMod = Path.Combine(mod.Path, path).Replace('\\', '/');
			foreach (var newFile in SystemUtils.GetAllFilesInFolderRecursive(pathInMod)) {
				if (foundFiles.ContainsKey(newFile)) {
					continue;
				}

				var fullPath = Path.Combine(pathInMod, newFile).Replace('\\', '/');
				foundFiles.Add(newFile, new(
					fromMod: true, newFile.Replace('\\', '/'), fullPath)
				);
			}

			if (PathIsReplaced(path, mod.ReplacedFolders)) {
				return foundFiles.Select(f => f.Value).ToList();
			}
		}

		var pathInGameRoot = Path.Combine(gameRoot, path).Replace('\\', '/');
		foreach (var newFile in SystemUtils.GetAllFilesInFolderRecursive(pathInGameRoot)) {
			if (foundFiles.ContainsKey(newFile)) {
				continue;
			}

			var fullPath = Path.Combine(pathInGameRoot, newFile).Replace('\\', '/');
			foundFiles.Add(newFile, new(
				fromMod: false, newFile.Replace('\\', '/'), fullPath)
			);
		}

		return foundFiles.Values.ToList();
	}

	public List<ModFSFileInfo> GetAllFilesInFolderRecursive(string path) {
		return GetAllFilesInFolderRecursive(path, precedenceComparer);
	}
	#endregion
}
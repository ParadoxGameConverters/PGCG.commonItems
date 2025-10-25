using commonItems.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Name = string;
using Path = string;

namespace commonItems.Mods;

public class Mod {
	public string Name { get; } = "";
	public string Path { get; } = "";
	public GameVersion? SupportedGameVersion { get; } = null;
	public ISet<string> Dependencies { get; } = new OrderedSet<string>();
	public ISet<string> ReplacedFolders { get; } = new OrderedSet<string>();
	public Mod() { }
	public Mod(string name, string path) {
		Name = name;
		Path = path;
	}
	public Mod(Name name, Path path, GameVersion? supportedGameVersion) : this(name, path) {
		SupportedGameVersion = supportedGameVersion;
	}
	public Mod(Name name, Path path, GameVersion? supportedGameVersion, IEnumerable<string> dependencies) : this(name, path, supportedGameVersion) {
		Dependencies = new OrderedSet<string>(dependencies);
	}
	public Mod(Name name, Path path, GameVersion? supportedGameVersion, IEnumerable<string> dependencies, ISet<string> replacedFolders) : this(name, path, supportedGameVersion, dependencies) {
		ReplacedFolders = replacedFolders;
	}
	public Mod(Name name, Path path, IEnumerable<string> dependencies) : this(name, path) {
		Dependencies = new OrderedSet<string>(dependencies);
	}
	public Mod(Name name, Path path, IEnumerable<string> dependencies, ISet<string> replacedFolders) : this(name, path, dependencies) {
		ReplacedFolders = replacedFolders;
	}

	public override bool Equals(object? obj) {
		return obj is Mod mod &&
			   Name == mod.Name &&
			   System.IO.Path.GetFullPath(Path) == System.IO.Path.GetFullPath(mod.Path);
	}
	public override int GetHashCode() {
		return HashCode.Combine(Name, Path);
	}
}
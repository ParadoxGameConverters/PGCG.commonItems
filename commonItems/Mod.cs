using System;
using System.Collections.Generic;

using Name = System.String;
using Path = System.String;

namespace commonItems;

public class Mod {
	public string Name { get; } = "";
	public string Path { get; } = "";
	public ISet<string> Dependencies { get; } = new SortedSet<string>();
	public Mod() { }
	public Mod(Name name, Path path) {
		Name = name;
		Path = path;
	}
	public Mod(Name name, Path path, ISet<Name> dependencies) {
		Name = name;
		Path = path;
		Dependencies = dependencies;
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
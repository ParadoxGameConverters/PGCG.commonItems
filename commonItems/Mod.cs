using System;
using System.Collections.Generic;

using Name = System.String;
using Path = System.String;

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

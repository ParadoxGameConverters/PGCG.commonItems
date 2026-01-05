namespace commonItems.Mods;

public struct ModFSFileInfo(bool fromMod, string relativePath, string absolutePath) {
	public bool FromMod = fromMod;
	public readonly string RelativePath = relativePath;
	public readonly string AbsolutePath = absolutePath;
}
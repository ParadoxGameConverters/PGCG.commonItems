namespace commonItems.Mods;

public struct ModFSFileInfo(bool fromMod, string relativePath, string absolutePath) {
	public bool FromMod = fromMod;
	public string RelativePath = relativePath;
	public string AbsolutePath = absolutePath;
}
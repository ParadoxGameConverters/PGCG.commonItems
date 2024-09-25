namespace commonItems.Mods;

public struct ModFSFileInfo(string relativePath, string absolutePath) {
	public string RelativePath = relativePath;
	public string AbsolutePath = absolutePath;
}
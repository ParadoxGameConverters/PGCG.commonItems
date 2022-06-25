using commonItems.Mods;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace commonItems.UnitTests.Mods;

public class ModFilesystemTests {
	[Fact]
	public void MissingFileReturnsNull() {
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root", new Mod[] { });

		var filePath = modFS.GetActualFileLocation("test_folder/non_file.txt");
		Assert.Null(filePath);
	}
	
	[Fact]
	public void FileCanBeFoundInGameRoot() {
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root", new Mod[] { });

		var filePath = modFS.GetActualFileLocation("test_folder/test_file.txt");
		Assert.NotNull(filePath);
		Assert.Equal("TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/test_file.txt", filePath);
	}
	
	[Fact]
	public void FileIsReplacedByMod() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root", new[] {modOne});

		var filePath = modFS.GetActualFileLocation("test_folder/test_file.txt");
		Assert.NotNull(filePath);
		Assert.Equal("TestFiles/ModFilesystem/GetActualFileLocation/mod_one/test_folder/test_file.txt", filePath);
	}
	
	[Fact]
	public void LatestModDeterminesFile() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modTwo = new Mod("Mod Two", "TestFiles/ModFilesystem/GetActualFileLocation/mod_two");
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root",
			new[] {modOne, modTwo});

		var filePath = modFS.GetActualFileLocation("test_folder/test_file.txt");
		Assert.NotNull(filePath);
		Assert.Equal("TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/test_file.txt", filePath);
	}
	
	[Fact]
	public void ModDoesNotReplaceFileIfFileNotInMod() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modTwo = new Mod("Mod Two", "TestFiles/ModFilesystem/GetActualFileLocation/mod_two");
		var modThree = new Mod("Mod Three", "TestFiles/ModFilesystem/GetActualFileLocation/mod_three");
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root",
			new[] {modOne, modTwo, modThree});

		var filePath = modFS.GetActualFileLocation("test_folder/test_file.txt");
		Assert.NotNull(filePath);
		Assert.Equal("TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/test_file.txt", filePath);
	}
	
	[Fact]
	public void ReplacePathBlocksEarlierInstancesOfFile() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modTwo = new Mod("Mod Two", "TestFiles/ModFilesystem/GetActualFileLocation/mod_two");
		var modThree = new Mod("Mod Three", "TestFiles/ModFilesystem/GetActualFileLocation/mod_three", new string[] { },
			new HashSet<string> {"test_folder"});
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root",
			new[] {modOne, modTwo, modThree});

		var filePath = modFS.GetActualFileLocation("test_folder/test_file.txt");
		Assert.Null(filePath);
	}
	
	[Fact]
	public void ReplacePathOnlyBlocksActualPath() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modTwo = new Mod("Mod Two", "TestFiles/ModFilesystem/GetActualFileLocation/mod_two");
		var modThree = new Mod("Mod Three", "TestFiles/ModFilesystem/GetActualFileLocation/mod_three", new string[] { },
			new HashSet<string> {"test_fold"});
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root",
			new[] {modOne, modTwo, modThree});

		var filePath = modFS.GetActualFileLocation("test_folder/test_file.txt");
		Assert.NotNull(filePath);
		Assert.Equal("TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/test_file.txt", filePath);
	}

	[Fact]
	public void MissingFolderReturnsNull() {
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root", new List<Mod>());

		var filePath = modFS.GetActualFolderLocation("test_folder/non_folder");
		Assert.Null(filePath);
	}
	
	[Fact]
	public void FolderCanBeFoundInGameRoot() {
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root", new List<Mod>());

		var filePath = modFS.GetActualFolderLocation("test_folder/deeper_folder");
		Assert.NotNull(filePath);
		Assert.Equal("TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/deeper_folder", filePath);
	}

	[Fact]
	public void FolderIsReplacedByMod() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root", new[] {modOne});

		var filePath = modFS.GetActualFolderLocation("test_folder/deeper_folder");
		Assert.NotNull(filePath);
		Assert.Equal("TestFiles/ModFilesystem/GetActualFileLocation/mod_one/test_folder/deeper_folder", filePath);
	}

	[Fact]
	public void LatestModDeterminesFolder() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modTwo = new Mod("Mod Two", "TestFiles/ModFilesystem/GetActualFileLocation/mod_two");
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root",
			new[] {modOne, modTwo});

		var filePath = modFS.GetActualFolderLocation("test_folder/deeper_folder");
		Assert.NotNull(filePath);
		Assert.Equal("TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/deeper_folder", filePath);
	}
	
	[Fact]
	public void ModDoesNotReplaceFolderIfFolderNotInMod() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modTwo = new Mod("Mod Two", "TestFiles/ModFilesystem/GetActualFileLocation/mod_two");
		var modThree = new Mod("Mod Three", "TestFiles/ModFilesystem/GetActualFileLocation/mod_three");
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root",
			new[] {modOne, modTwo, modThree});

		var filePath = modFS.GetActualFolderLocation("test_folder/deeper_folder");
		Assert.NotNull(filePath);
		Assert.Equal("TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/deeper_folder", filePath);
	}
	
	[Fact]
	public void ReplacePathBlocksEarlierInstancesOfFolder() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modTwo = new Mod("Mod Two", "TestFiles/ModFilesystem/GetActualFileLocation/mod_two");
		var modThree = new Mod("Mod Three", "TestFiles/ModFilesystem/GetActualFileLocation/mod_three", new string[] { },
			new HashSet<string> {"test_folder/"});
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root",
			new[] {modOne, modTwo, modThree});

		var filePath = modFS.GetActualFolderLocation("test_folder/deeper_folder");
		Assert.Null(filePath);
	}
	
	[Fact]
	public void NoFilesInMissingDirectory() {
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root", new List<Mod>());

		modFS.GetAllFilesInFolder("/non_folder").Should().BeEmpty();
	}

	[Fact]
	public void FilesInGameRootAreFound() {
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root", new List<Mod>());

		modFS.GetAllFilesInFolder("test_folder").Should().Equal(
			"TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/root_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/test_file.txt");
	}

	[Fact]
	public void ModFilesAddToAndReplaceGameRootFiles() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root", new[] {modOne});

		modFS.GetAllFilesInFolder("test_folder").Should().Equal(
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_one/test_folder/mod_one_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/root_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_one/test_folder/test_file.txt");
	}
	
	[Fact]
	public void ModFilesAddToAndReplaceEarlierModFiles() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modTwo = new Mod("Mod Two", "TestFiles/ModFilesystem/GetActualFileLocation/mod_two");
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root",
			new[] {modOne, modTwo});

		modFS.GetAllFilesInFolder("test_folder").Should().Equal(
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_one/test_folder/mod_one_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/mod_two_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/root_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/test_file.txt");
	}
	
	[Fact]
	public void ReplaceFolderKeepsFilesFromBeingFound() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modTwo = new Mod("Mod Two", "TestFiles/ModFilesystem/GetActualFileLocation/mod_two", new string[] { },
			new HashSet<string>() {"test_folder"});
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root",
			new[] {modOne, modTwo});

		modFS.GetAllFilesInFolder("test_folder").Should().Equal(
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/mod_two_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/test_file.txt");
	}

	private class CustomPrecedenceComparer : IComparer<string> {
		// longest path first
		public int Compare(String? x, String? y) {
			var xLength = x!.Length;
			var yLength = y!.Length;
			if (xLength > yLength) {
				return -1;
			}
			return xLength == yLength ? 0 : 1;
		}
	}

	[Fact]
	public void GetAllFilesInFolderCanBeCalledWithCustomFilePrecedenceComparer() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modTwo = new Mod("Mod Two", "TestFiles/ModFilesystem/GetActualFileLocation/mod_two", new string[] { },
			new HashSet<string>() {"test_folder"});
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root",
			new[] {modOne, modTwo});

		modFS.GetAllFilesInFolder("test_folder", new CustomPrecedenceComparer()).Should().Equal(
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/mod_two_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/test_file.txt");
	}
	
	[Fact]
	public void NoFoldersInMissingDirectory() {
		var modFileSystem =
			new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root", new List<Mod>());

		modFileSystem.GetAllSubfolders("/non_folder").Should().BeEmpty();
	}
	
	[Fact]
	public void FoldersInGameRootAreFound() {
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root", new List<Mod>());

		modFS.GetAllSubfolders("test_folder").Should().Equal(
			"TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/deeper_folder",
			"TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/game_folder");
	}

	[Fact]
	public void ModFoldersAddToAndReplaceGameRootFolders() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root", new[] {modOne});

		modFS.GetAllSubfolders("test_folder").Should().Equal(
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_one/test_folder/deeper_folder",
			"TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/game_folder",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_one/test_folder/mod_one_folder");
	}

	[Fact]
	public void ModFoldersAddToAndReplaceEarlierModFolders() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modTwo = new Mod("Mod Two", "TestFiles/ModFilesystem/GetActualFileLocation/mod_two");
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root",
			new[] {modOne, modTwo});

		modFS.GetAllSubfolders("test_folder").Should().Equal(
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/deeper_folder",
			"TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/game_folder",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_one/test_folder/mod_one_folder",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/mod_two_folder");
	}
	
	[Fact]
	public void ReplaceFolderKeepsFoldersFromBeingFound() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modTwo = new Mod("Mod Two", "TestFiles/ModFilesystem/GetActualFileLocation/mod_two", new string[] { },
			new HashSet<string> {"test_folder"});
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root",
			new[] {modOne, modTwo});

		modFS.GetAllSubfolders("test_folder").Should().Equal(
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/deeper_folder",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/mod_two_folder");
	}

	[Fact]
	public void GetAllSubfoldersCanBeCalledWithCustomFolderPrecedenceComparer() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modTwo = new Mod("Mod Two", "TestFiles/ModFilesystem/GetActualFileLocation/mod_two", new string[] { },
			new HashSet<string> {"test_folder"});
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root",
			new[] {modOne, modTwo});

		modFS.GetAllSubfolders("test_folder", new CustomPrecedenceComparer()).Should().Equal(
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/mod_two_folder",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/deeper_folder");
	}
	
	[Fact]
	public void NoFilesInMissingDirectoryTree() {
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root", new List<Mod>());

		modFS.GetAllFilesInFolderRecursive("/non_folder").Should().BeEmpty();
	}

	[Fact]
	public void FilesInGameRootAndSubfoldersAreFound() {
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root", new List<Mod>());

		modFS.GetAllFilesInFolderRecursive("test_folder").Should().Equal(
			"TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/root_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/test_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/deeper_folder/dummy.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/game_folder/dummy.txt");
	}

	[Fact]
	public void ModFilesAndSubfoldersAddToAndReplaceGameRootFiles() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root", new[] {modOne});

		modFS.GetAllFilesInFolderRecursive("test_folder").Should().Equal(
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_one/test_folder/mod_one_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/root_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_one/test_folder/test_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_one/test_folder/deeper_folder/dummy.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/game_folder/dummy.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_one/test_folder/mod_one_folder/dummy.txt");
	}
	
	[Fact]
	public void ModFilesAndSubfoldersAddToAndReplaceEarlierModFiles() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modTwo = new Mod("Mod Two", "TestFiles/ModFilesystem/GetActualFileLocation/mod_two");
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root",
			new[] {modOne, modTwo});

		modFS.GetAllFilesInFolderRecursive("test_folder").Should().Equal(
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_one/test_folder/mod_one_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/mod_two_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/root_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/test_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/deeper_folder/dummy.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/game_root/test_folder/game_folder/dummy.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_one/test_folder/mod_one_folder/dummy.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/mod_two_folder/dummy.txt");
	}

	[Fact]
	public void ReplaceFolderKeepsFilesAndSubfoldersFromBeingFound() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modTwo = new Mod("Mod Two", "TestFiles/ModFilesystem/GetActualFileLocation/mod_two", new string[] { },
			new HashSet<string> {"test_folder"});
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root",
			new[] {modOne, modTwo});

		modFS.GetAllFilesInFolderRecursive("test_folder").Should().Equal(
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/mod_two_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/test_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/deeper_folder/dummy.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/mod_two_folder/dummy.txt");
	}

	[Fact]
	public void GetAllFilesInFolderRecursiveCanBeCalledWithCustomFilePrecedenceComparer() {
		var modOne = new Mod("Mod One", "TestFiles/ModFilesystem/GetActualFileLocation/mod_one");
		var modTwo = new Mod("Mod Two", "TestFiles/ModFilesystem/GetActualFileLocation/mod_two", new string[] { },
			new HashSet<string> {"test_folder"});
		var modFS = new ModFilesystem("TestFiles/ModFilesystem/GetActualFileLocation/game_root",
			new[] {modOne, modTwo});

		modFS.GetAllFilesInFolderRecursive("test_folder", new CustomPrecedenceComparer()).Should().Equal(
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/mod_two_folder/dummy.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/deeper_folder/dummy.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/mod_two_file.txt",
			"TestFiles/ModFilesystem/GetActualFileLocation/mod_two/test_folder/test_file.txt");
	}
}
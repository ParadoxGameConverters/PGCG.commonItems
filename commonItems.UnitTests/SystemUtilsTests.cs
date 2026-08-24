using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace commonItems.UnitTests;

[Collection("Sequential")]
[CollectionDefinition("Sequential", DisableParallelization = true)]
public sealed class SystemUtilsTests {
	private const string TestFilesPath = "TestFiles/SystemUtilsTestFiles";

	[Fact]
	public void GetAllFilesInFolderDoesNotWorkRecursively() {
		var files = SystemUtils.GetAllFilesInFolder(TestFilesPath);
		var expected = new SortedSet<string>{
			"keyValuePair.txt",
		};
		Assert.Equal(expected, files);
	}
	[Fact]
	public void GetAllFilesInFolderReturnsEmptySetOnNonexistentFolder() {
		var files = SystemUtils.GetAllFilesInFolder($"{TestFilesPath}/missingDir");
		Assert.Empty(files);
	}
	[Fact]
	public void GetAllFilesInFolderRecursiveWorkRecursively() {
		var files = SystemUtils.GetAllFilesInFolderRecursive(TestFilesPath);
		var expected = new SortedSet<string>{
			"keyValuePair.txt",
			Path.Combine("subfolder", "subfolder_file.txt"),
			Path.Combine("subfolder", "subfolder_file2.txt"),
			Path.Combine("subfolder2", "subfolder2_file.txt"),
		};
		Assert.Equal(expected, files);
	}
	[Fact]
	public void GetAllFilesInFolderRecursiveReturnsEmptySetOnNonexistentFolder() {
		var files = SystemUtils.GetAllFilesInFolderRecursive($"{TestFilesPath}/missingDir");
		Assert.Empty(files);
	}
	[Fact]
	public void GetAllSubfoldersGetsSubfolders() {
		var subfolders = SystemUtils.GetAllSubfolders(TestFilesPath);
		var expected = new SortedSet<string>{
			"subfolder",
			"subfolder2",
		};
		Assert.Equal(expected, subfolders);
	}
	[Fact]
	public void GetAllSubfoldersReturnsEmptySetOnNonexistentFolder() {
		var subfolders = SystemUtils.GetAllSubfolders($"{TestFilesPath}/missingDir");
		Assert.Empty(subfolders);
	}
	[Fact]
	public void TryCreateFolderCreatesFolder() {
		var created = SystemUtils.TryCreateFolder($"{TestFilesPath}/newFolder");
		Assert.True(created);
		Assert.True(Directory.Exists($"{TestFilesPath}/newFolder"));
		Directory.Delete($"{TestFilesPath}/newFolder", recursive: true); // cleanup
	}
	[Fact]
	public void TryCreateFolderLogsErrorOnImpossiblePath() {
		var output = new StringWriter();
		Console.SetOut(output);

		const string path = "/!@#$%^&*()<>/?aęóź??@#$SFGsf65gfh";
		var created = SystemUtils.TryCreateFolder(path);
		Assert.False(created);
		Assert.False(Directory.Exists(path));
		Assert.Contains(
			$"[ERROR] Could not create directory: \"{path}\": ",
			output.ToString());
	}
	[Fact]
	public void TryCreateFolderLogsErrorOnEmptyPath() {
		var output = new StringWriter();
		Console.SetOut(output);

		const string path = "";
		var created = SystemUtils.TryCreateFolder(path);
		Assert.False(created);
		Assert.False(Directory.Exists(path));
		Assert.Contains(
			$"[ERROR] Could not create directory: \"{path}\": Path is empty or whitespace.",
			output.ToString());
	}
	[Fact]
	public void TryCreateFolderLogsErrorOnWhitespacePath() {
		var output = new StringWriter();
		Console.SetOut(output);

		const string path = "   ";
		var created = SystemUtils.TryCreateFolder(path);
		Assert.False(created);
		Assert.False(Directory.Exists(path));
		Assert.Contains(
			$"[ERROR] Could not create directory: \"{path}\": Path is empty or whitespace.",
			output.ToString());
	}

	[Fact]
	public void TryCopyFileCopiesFile() {
		const string sourcePath = $"{TestFilesPath}/subfolder2/subfolder2_file.txt";
		const string destPath = $"{TestFilesPath}/subfolder/subfolder2_file.txt";
		var success = SystemUtils.TryCopyFile(sourcePath, destPath);
		Assert.True(success);
		Assert.True(File.Exists(destPath));
		File.Delete(destPath); // cleanup
	}
	[Fact]
	public void TryCopyFileLogsErrorOnMissingSourceFile() {
		var output = new StringWriter();
		Console.SetOut(output);

		const string sourcePath = $"{TestFilesPath}/subfolder/missingFile.txt";
		const string destPath = $"{TestFilesPath}/newFolder/file.txt";
		var success = SystemUtils.TryCopyFile(sourcePath, destPath);
		Assert.False(success);
		Assert.False(File.Exists(destPath));
		Assert.Contains($"[WARN] Could not copy file \"{sourcePath}\" to \"{destPath}\" " +
		                "- System.IO.FileNotFoundException: Could not find file",
			output.ToString());
	}

	[Fact]
	public void CopyFolderCopiesFolder() {
		const string sourcePath = $"{TestFilesPath}/subfolder2";
		const string destPath = $"{TestFilesPath}/subfolder3";
		Assert.False(Directory.Exists(destPath));
		var success = SystemUtils.TryCopyFolder(sourcePath, destPath);
		Assert.True(success);
		Assert.True(Directory.Exists(destPath));
		Directory.Delete(destPath, recursive: true); // cleanup
	}
	[Fact]
	public void CopyFolderLogsErrorOnMissingSourceFolder() {
		var output = new StringWriter();
		Console.SetOut(output);

		const string sourcePath = $"{TestFilesPath}/missingFolder";
		const string destPath = $"{TestFilesPath}/newFolder";
		var success = SystemUtils.TryCopyFolder(sourcePath, destPath);
		Assert.False(success);
		Assert.False(Directory.Exists(destPath));
		Assert.Contains("[ERROR] Could not copy folder: " +
		                "System.IO.DirectoryNotFoundException: Source directory does not exist or could not be found",
			output.ToString());
	}

	[Fact]
	public void CopyFolderCopiesSubfoldersRecursively() {
		const string sourcePath = $"{TestFilesPath}/recursiveSource";
		const string destPath = $"{TestFilesPath}/recursiveDest";
		try {
			Assert.False(Directory.Exists(sourcePath));
			Assert.False(Directory.Exists(destPath));

			// Prebuilt test structure: source with a file and a nested subfolder with a file.
			var nestedDir = Path.Combine(sourcePath, "nestedSubfolder");
			Directory.CreateDirectory(nestedDir);
			var sourceFile = Path.Combine(sourcePath, "sourceFile.txt");
			var nestedFile = Path.Combine(nestedDir, "nestedFile.txt");
			File.WriteAllText(sourceFile, "source");
			File.WriteAllText(nestedFile, "nested");

			var success = SystemUtils.TryCopyFolder(sourcePath, destPath);
			Assert.True(success);
			Assert.True(Directory.Exists(destPath));
			Assert.False(Directory.Exists($"{destPath}/missing"));

			// Files in root of source are copied.
			Assert.True(File.Exists(Path.Combine(destPath, "sourceFile.txt")));
			Assert.Equal("source", File.ReadAllText(Path.Combine(destPath, "sourceFile.txt")));

			// Nested subfolder is copied along with its files.
			Assert.True(Directory.Exists(Path.Combine(destPath, "nestedSubfolder")));
			Assert.True(File.Exists(Path.Combine(destPath, "nestedSubfolder", "nestedFile.txt")));
			Assert.Equal("nested", File.ReadAllText(Path.Combine(destPath, "nestedSubfolder", "nestedFile.txt")));
		} finally {
			if (Directory.Exists(sourcePath)) {
				Directory.Delete(sourcePath, recursive: true);
			}
			if (Directory.Exists(destPath)) {
				Directory.Delete(destPath, recursive: true);
			}
		}
	}

	[Fact]
	public void RenameFolderRenamesFolder() {
		const string path = $"{TestFilesPath}/subfolder2";
		const string newPath = $"{TestFilesPath}/subfolderRenamed";
		Assert.True(Directory.Exists(path));
		Assert.False(Directory.Exists(newPath));
		var success = SystemUtils.TryRenameFolder(path, newPath);
		Assert.True(success);
		Assert.False(Directory.Exists(path));
		Assert.True(Directory.Exists(newPath));
		SystemUtils.TryRenameFolder(newPath, path); // cleanup
	}
	[Fact]
	public void RenameFolderLogsErrorOnMissingSourceFolder() {
		var output = new StringWriter();
		Console.SetOut(output);

		const string sourcePath = $"{TestFilesPath}/missingFolder";
		const string destPath = $"{TestFilesPath}/newFolder";
		var success = SystemUtils.TryRenameFolder(sourcePath, destPath);
		Assert.False(success);
		Assert.False(Directory.Exists(destPath));
		var outputString = output.ToString();
		// The exact exception message for Directory.Move on a missing source is platform-specific.
		Assert.Contains("[ERROR] Could not rename folder: ", outputString);
		if (OperatingSystem.IsWindows()) {
			Assert.Contains("System.IO.DirectoryNotFoundException: Could not find a part of the path", outputString);
		}
	}

	[Fact]
	public void DeleteFolderDeletesFolder() {
		const string path = $"{TestFilesPath}/tempFolder";
		SystemUtils.TryCreateFolder(path);
		Assert.True(Directory.Exists(path));
		var success = SystemUtils.TryDeleteFolder(path);
		Assert.True(success);
		Assert.False(Directory.Exists(path));
	}
	[Fact]
	public void DeleteFolderLogsErrorOnMissingSourceFolder() {
		var output = new StringWriter();
		Console.SetOut(output);

		const string path = $"{TestFilesPath}/missingFolder";
		var success = SystemUtils.TryDeleteFolder(path);
		Assert.False(success);
		var outputString = output.ToString();
		// The exact exception message for Directory.Delete on a missing source is platform-specific.
		Assert.Contains(
			$"[ERROR] Could not delete folder: \"{path}\": ",
			outputString);
		if (OperatingSystem.IsWindows()) {
			Assert.Contains("System.IO.DirectoryNotFoundException: Could not find a part of the path", outputString);
		}
	}
}
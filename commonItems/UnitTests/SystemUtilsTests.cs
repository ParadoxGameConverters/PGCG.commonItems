using System;
using System.IO;
using Xunit;

namespace commonItems.UnitTests {
    [Collection("Sequential")]
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class SystemUtilsTests {
        readonly string testFilesPath = "UnitTests/TestFiles";
        [Fact] public void GetAllFilesInFolderDoesntWorkRecursively() {
            var files = SystemUtils.GetAllFilesInFolder(testFilesPath);
            Assert.Single(files);
        }
        [Fact]
        public void GetAllFilesInFolderReturnsEmptySetOnNonexistentFolder() {
            var files = SystemUtils.GetAllFilesInFolder(testFilesPath + "/missingDir");
            Assert.Empty(files);
        }
        [Fact]
        public void GetAllFilesInFolderRecursiveWorkRecursively() {
            var files = SystemUtils.GetAllFilesInFolderRecursive(testFilesPath);
            Assert.Equal(4, files.Count);
        }
        [Fact]
        public void GetAllFilesInFolderRecursiveReturnsEmptySetOnNonexistentFolder() {
            var files = SystemUtils.GetAllFilesInFolderRecursive(testFilesPath + "/missingDir");
            Assert.Empty(files);
        }
        [Fact]
        public void GetAllSubfoldersGetsSubfolders() {
            var subfolders = SystemUtils.GetAllSubfolders(testFilesPath);
            Assert.Equal(2, subfolders.Count);
        }
        [Fact]
        public void GetAllSubfoldersReturnsEmptySetOnNonexistentFolder() {
            var subfolders = SystemUtils.GetAllSubfolders(testFilesPath + "/missingDir");
            Assert.Empty(subfolders);
        }
        [Fact]
        public void TryCreateFolderCreatesFolder() {
            var created = SystemUtils.TryCreateFolder(testFilesPath + "/newFolder");
            Assert.True(created);
            Assert.True(Directory.Exists(testFilesPath + "/newFolder"));
            Directory.Delete(testFilesPath + "/newFolder", recursive: true); // cleanup
        }
        [Fact] public void TryCreateFolderLogsErrorOnEmptyPath() {
            var output = new StringWriter();
            Console.SetOut(output);

            var path = "";
            var created = SystemUtils.TryCreateFolder(path);
            Assert.False(created);
            Assert.False(Directory.Exists(path));
            Assert.StartsWith("    [ERROR] Could not create directory: " + path +
                " : System.ArgumentException: Path cannot be the empty string or all whitespace. (Parameter 'path')",
                output.ToString());
        }

        [Fact] public void TryCopyFileCopiesFile() {
            var sourcePath = testFilesPath + "/subfolder2/subfolder2_file.txt";
            var destPath = testFilesPath + "/subfolder/subfolder2_file.txt";
            var success = SystemUtils.TryCopyFile(sourcePath, destPath);
            Assert.True(success);
            Assert.True(File.Exists(destPath));
            File.Delete(destPath); // cleanup
        }
        [Fact]
        public void TryCopyFileLogsErrorOnMissingSourceFile() {
            var output = new StringWriter();
            Console.SetOut(output);

            var sourcePath = testFilesPath + "/subfolder/missingFile.txt";
            var destPath = testFilesPath + "/newFolder/file.txt";
            var success = SystemUtils.TryCopyFile(sourcePath, destPath);
            Assert.False(success);
            Assert.False(File.Exists(destPath));
            Assert.StartsWith("  [WARNING] Could not copy file " + sourcePath +
                " to " + destPath + " - System.IO.FileNotFoundException: Could not find file",
                output.ToString());
        }

        [Fact]
        public void CopyFolderCopiesFolder() {
            var sourcePath = testFilesPath + "/subfolder2";
            var destPath = testFilesPath + "/subfolder3";
            Assert.False(Directory.Exists(destPath));
            var success = SystemUtils.CopyFolder(sourcePath, destPath);
            Assert.True(success);
            Assert.True(Directory.Exists(destPath));
            Directory.Delete(destPath, recursive: true); // cleanup
        }
        [Fact]
        public void CopyFolderLogsErrorOnMissingSourceFolder() {
            var output = new StringWriter();
            Console.SetOut(output);

            var sourcePath = testFilesPath + "/missingFolder";
            var destPath = testFilesPath + "/newFolder";
            var success = SystemUtils.CopyFolder(sourcePath, destPath);
            Assert.False(success);
            Assert.False(Directory.Exists(destPath));
            Assert.StartsWith("    [ERROR] Could not copy folder: " +
                "System.IO.DirectoryNotFoundException: Source directory does not exist or could not be found: UnitTests/TestFiles/missingFolder",
                output.ToString());
        }

        [Fact]
        public void RenameFolderRenamesFolder() {
            var path = testFilesPath + "/subfolder2";
            var newPath = testFilesPath + "/subfolderRenamed";
            Assert.True(Directory.Exists(path));
            Assert.False(Directory.Exists(newPath));
            var success = SystemUtils.RenameFolder(path, newPath);
            Assert.True(success);
            Assert.False(Directory.Exists(path));
            Assert.True(Directory.Exists(newPath));
            SystemUtils.RenameFolder(newPath, path); // cleanup
        }
        [Fact]
        public void RenameFolderLogsErrorOnMissingSourceFolder() {
            var output = new StringWriter();
            Console.SetOut(output);

            var sourcePath = testFilesPath + "/missingFolder";
            var destPath = testFilesPath + "/newFolder";
            var success = SystemUtils.RenameFolder(sourcePath, destPath);
            Assert.False(success);
            Assert.False(Directory.Exists(destPath));
            Assert.StartsWith("    [ERROR] Could not rename folder: " +
                "System.IO.DirectoryNotFoundException: Could not find a part of the path",
                output.ToString());
        }

        [Fact] public void DeleteFolderDeletesFolder() {
            var path = testFilesPath + "/tempFolder";
            SystemUtils.TryCreateFolder(path);
            Assert.True(Directory.Exists(path));
            var success = SystemUtils.DeleteFolder(path);
            Assert.True(success);
            Assert.False(Directory.Exists(path));
        }
        [Fact]
        public void DeleteFolderLogsErrorOnMissingSourceFolder() {
            var output = new StringWriter();
            Console.SetOut(output);

            var path = testFilesPath + "/missingFolder";
            var success = SystemUtils.DeleteFolder(path);
            Assert.False(success);
            Assert.StartsWith("    [ERROR] Could not delete folder: " + path + " : " + 
                "System.IO.DirectoryNotFoundException: Could not find a part of the path",
                output.ToString());
        }
    }
}

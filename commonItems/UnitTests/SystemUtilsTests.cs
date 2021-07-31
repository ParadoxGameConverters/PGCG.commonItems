using System.IO;
using Xunit;

namespace commonItems.UnitTests {
    [Collection("Sequential")]
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class SystemUtilsTests {
        string testFilesPath = "UnitTests/TestFiles";
        [Fact] public void GetAllFilesInFolderDoesntWorkRecursively() {
            var files = SystemUtils.GetAllFilesInFolder(testFilesPath);
            Assert.Single(files);
        }
        [Fact]
        public void GetAllFilesInFolderRecursiveWorkRecursively() {
            var files = SystemUtils.GetAllFilesInFolderRecursive(testFilesPath);
            Assert.Equal(4, files.Count);
        }
        [Fact]
        public void GetAllSubfoldersGetsSubfolders() {
            var subfolders = SystemUtils.GetAllSubfolders(testFilesPath);
            Assert.Equal(2, subfolders.Count);
        }
        [Fact]
        public void TryCreateFolderCreatesFolder() {
            var created = SystemUtils.TryCreateFolder(testFilesPath + "/newFolder");
            Assert.True(created);
            Assert.True(Directory.Exists(testFilesPath + "/newFolder"));
            Directory.Delete(testFilesPath + "/newFolder", recursive: true); // cleanup
        }

        [Fact] public void TryCopyFileCopiesFile() {
            var sourcePath = testFilesPath + "/subfolder2/subfolder2_file.txt";
            var destPath = testFilesPath + "/subfolder/subfolder2_file.txt";
            var success = SystemUtils.TryCopyFile(sourcePath, destPath);
            Assert.True(success);
            Assert.True(SystemUtils.DoesFileExist(destPath));
            File.Delete(destPath); // cleanup
        }

        [Fact]
        public void CopyFolderCopiesFolder() {
            var sourcePath = testFilesPath + "/subfolder2";
            var destPath = testFilesPath + "/subfolder3";
            Assert.False(SystemUtils.DoesFolderExist(destPath));
            var success = SystemUtils.CopyFolder(sourcePath, destPath);
            Assert.True(success);
            Assert.True(SystemUtils.DoesFolderExist(destPath));
            Directory.Delete(destPath, recursive: true); // cleanup
        }

        [Fact]
        public void RenameFolderRenamesFolder() {
            var path = testFilesPath + "/subfolder2";
            var newPath = testFilesPath + "/subfolderRenamed";
            Assert.True(SystemUtils.DoesFolderExist(path));
            Assert.False(SystemUtils.DoesFolderExist(newPath));
            var success = SystemUtils.RenameFolder(path, newPath);
            Assert.True(success);
            Assert.False(SystemUtils.DoesFolderExist(path));
            Assert.True(SystemUtils.DoesFolderExist(newPath));
            SystemUtils.RenameFolder(newPath, path); // cleanup
        }

        [Fact] public void DeleteFolderDeletesFolder() {
            var path = testFilesPath + "/tempFolder";
            SystemUtils.TryCreateFolder(path);
            Assert.True(SystemUtils.DoesFolderExist(path));
            var success = SystemUtils.DeleteFolder(path);
            Assert.True(success);
            Assert.False(SystemUtils.DoesFolderExist(path));
        }
    }
}

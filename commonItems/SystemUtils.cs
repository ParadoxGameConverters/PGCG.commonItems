using System;
using System.Collections.Generic;
using System.IO;

namespace commonItems {
    public static class SystemUtils {
        public static SortedSet<string> GetAllFilesInFolder(string path) {
            if (DoesFolderExist(path)) {
                return new SortedSet<string>(Directory.GetFiles(path));
            }
            return new SortedSet<string>();
        }
        public static SortedSet<string> GetAllFilesInFolderRecursive(string path) {
            if (DoesFolderExist(path)) {
                return new SortedSet<string>(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories));
            }
            return new SortedSet<string>();
        }

        public static SortedSet<string> GetAllSubfolders(string path) {
            if (DoesFolderExist(path)) {
                return new SortedSet<string>(Directory.GetDirectories(path));
            }
            return new SortedSet<string>();
        }

        public static bool TryCreateFolder(string path) {
            try {
                return Directory.CreateDirectory(path).Exists;
            } catch (Exception e) {
                Logger.Log(LogLevel.Error, "Could not create directory: " + path + " : " + e.ToString());
                return false;
            }
        }

        public static bool TryCopyFile(string sourcePath, string destPath) {
            try {
                File.Copy(sourcePath, destPath);
                return true;
            } catch (Exception e) {
                Logger.Log(LogLevel.Warning, "Could not copy file " + sourcePath + " to " + destPath + " - " + e.ToString());
                return false;
            }
        }

        public static bool CopyFolder(string sourceFolder, string destFolder) {
            // https://docs.microsoft.com/pl-pl/dotnet/standard/io/how-to-copy-directories
            try {
                // Get the subdirectories for the specified directory.
                var dir = new DirectoryInfo(sourceFolder);

                if (!dir.Exists) {
                    throw new DirectoryNotFoundException(
                        "Source directory does not exist or could not be found: "
                        + sourceFolder);
                }

                DirectoryInfo[] dirs = dir.GetDirectories();

                // If the destination directory doesn't exist, create it.       
                Directory.CreateDirectory(destFolder);

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files) {
                    string tempPath = Path.Combine(destFolder, file.Name);
                    file.CopyTo(tempPath, false);
                }

                // Copy subdirectories and their contents to new location.
                foreach (DirectoryInfo subdir in dirs) {
                    string tempPath = Path.Combine(destFolder, subdir.Name);
                    CopyFolder(subdir.FullName, tempPath);
                }

                return true;
            } catch (Exception e) {
                Logger.Log(LogLevel.Error, "Could not rename folder: " + e.ToString());
                return false;
            }
        }

        public static bool RenameFolder(string sourceFolder, string destFolder) {
            try {
                Directory.Move(sourceFolder, destFolder);
                return true;
            } catch (Exception e) {
                Logger.Log(LogLevel.Error, "Could not rename folder: " + e.ToString());
                return false;
            }
        }

        public static bool DoesFileExist(string path) {
            return File.Exists(path);
        }

        public static bool DoesFolderExist(string path) {
            return Directory.Exists(path);
        }

        public static bool DeleteFolder(string folder) {
            try {
                Directory.Delete(folder, recursive: true);
                return true;
            } catch (Exception e) {
                Logger.Log(LogLevel.Error, "Could not delete folder: " + folder + " : " + e.ToString());
                return false;
            }
        }
    }
}

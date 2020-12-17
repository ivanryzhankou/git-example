using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace lab_02.DataLayer
{
    class DataRepository
    {
        public const long maxFileSize = 157286400; // 150 Megabyte
        public const long MaximumStorageSize = 10737418240; // 10 Gigabyte
        
        internal void DeleteFileFromStorage(string pathToFile)
        {
            File.Delete(pathToFile);
        }

        public void RenameFile(string originalName, string newName)
        {
            File.Move(originalName, newName);
        }

        internal void UploadFilesIntoStorage(string pathToFile)
        {
            var fileInf = new FileInfo(pathToFile);
            fileInf.CopyTo((ConfigurationManager.AppSettings.Get("storageAddress") + "\\" + fileInf.Name));
        }

        internal void DownloadFilesIntoStorage(string unloadingFile, string pathToUnloadingFile)
        {
            File.Copy(unloadingFile, pathToUnloadingFile, true);
        }

        internal bool IsFileExistence(string pathToFile) //b
        {
            return File.Exists(pathToFile);
        }

        internal bool CheckStorageOverflow(string pathToFile) //b
        {
            return GetFolderSize(ConfigurationManager.AppSettings.Get("storageAddress")) + GetFileSize(pathToFile) > MaximumStorageSize;
        }

        internal long GetFileSize(string pathToFile) //b
        {
            FileInfo File = new FileInfo(pathToFile);

            return File.Length;
        }

        internal bool CheckOnMaxSizeFile(string pathToFile) //b
        {
            return GetFileSize(pathToFile) > maxFileSize;
        }

        internal long GetFolderSize(string pathToFolder) //b
        {
            List<string> files = new List<string>(Directory.GetFiles(pathToFolder));

            return files.Select(x => x.Length).Sum();
        }

        internal bool IsFileNameUnique(string pathToFile, string pathToFolder) //b
        {
            var files = new List<string>(Directory.GetFiles(pathToFolder));
            var File = new FileInfo(pathToFile);

            for (int i = 0; i < files.Count; i++)
            {
                if (File.Name == (files[i].Remove(0, (pathToFolder.Length + 1))))
                {
                    return false;
                }
            }
            return true;
        }

        internal bool СheckUniquenessFolderName(string storageName, string pathToFolder) //b
        {
            List<string> directorys = new List<string>(Directory.GetDirectories(pathToFolder));
            DirectoryInfo directoryInfo = new DirectoryInfo(storageName);

            for (int i = 0; i < directorys.Count; i++)
            {
                if (directoryInfo.Name == (directorys[i].Remove(0, (pathToFolder.Length + 1))))
                {
                    return false;
                }
            }
            return true;
        }

        internal void CreateDirectory (string pathToDirectory)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(pathToDirectory);
            dirInfo.Create();
        }
    }
}


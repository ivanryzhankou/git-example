using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Linq;//todo: don't forget to remove usings you don't need

namespace lab_02.DataLayer
{
    class DataRepository
    {
        //todo: naming for public fields
        public long maxFileSize = 157286400; // 150 Megabyte
        public long MaximumStorageSize = 10737418240; // 10 Gigabyte

        
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
            //todo: var here would be better
            FileInfo fileInf = new FileInfo(pathToFile);
            fileInf.CopyTo((ConfigurationManager.AppSettings.Get("storageAddress") + "\\" + fileInf.Name));
        }

        internal void UploadFilesIntoStorge(string pathToFile)
        {
            FileInfo fileInf = new FileInfo(pathToFile);
            fileInf.CopyTo((ConfigurationManager.AppSettings.Get("storageAddress") + "\\" + fileInf.Name));
        }
        //todo: Storge -  typo
        //todo: unload - don't know this word
        internal void UnloadFilesIntoStorge(string unloadingFile, string pathToUnloadingFile)
        {
            File.Copy(unloadingFile, pathToUnloadingFile, true);
        }

        internal bool IsFileExistence(string pathToFile)
        {
            return File.Exists(pathToFile);
        }

        //todo: naming method
        internal bool checkOnStorageOverflow(string pathToFile)
        {
            return GetFolderSize(ConfigurationManager.AppSettings.Get("storageAddress")) + GetFileSize(pathToFile) > MaximumStorageSize;
        }

        internal long GetFileSize(string pathToFile)
        {
            FileInfo File = new FileInfo(pathToFile);

            return File.Length;
        }

        internal bool CheckOnMaxSizeFile(string pathToFile)
        {
            return GetFileSize(pathToFile) > maxFileSize;
        }

        internal long GetFolderSize(string pathToFolder)
        {
            //todo: I'm not sure, but I think the whole method can be replaced by one line: files.Select(x => x.Length).Sum();
            long folderSize = 0;

            List<string> files = new List<string>(Directory.GetFiles(pathToFolder));

            foreach (string file in files)
            {
                folderSize += file.Length;
            }
            return (folderSize);
        }

        //todo: naming like IsFileNameUnique would be better IMHO
        internal bool СheckUniquenessFilename(string pathToFile, string pathToFolder)
        {
            //todo: use var if you create a variable using new
            List<string> files = new List<string>(Directory.GetFiles(pathToFolder));
            FileInfo File = new FileInfo(pathToFile);

            //todo: hard to understand what is happening here. 
            for (int i = 0; i < files.Count; i++)
            {
                if (File.Name == (files[i].Remove(0, (pathToFolder.Length + 1))))
                {
                    return false;
                }
            }
            return true;
        }

        internal bool СheckUniquenessFolderName(string storageName, string pathToFolder)
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


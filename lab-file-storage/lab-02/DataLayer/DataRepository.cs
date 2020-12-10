using System;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;

namespace lab_02.DataLayer
{
    class DataRepository
    {
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

            FileInfo fileInf = new FileInfo(pathToFile);
            fileInf.CopyTo((ConfigurationManager.AppSettings.Get("storageAddress") + "\\" + fileInf.Name));
        }

        internal void UploadFilesIntoStorge(string pathToFile)
        {
            FileInfo fileInf = new FileInfo(pathToFile);
            fileInf.CopyTo((ConfigurationManager.AppSettings.Get("storageAddress") + "\\" + fileInf.Name));
        }

        internal void UnloadFilesIntoStorge(string unloadingFile, string pathToUnloadingFile)
        {
            File.Copy(unloadingFile, pathToUnloadingFile, true);
        }

        internal bool IsFileExistence(string pathToFile)
        {
            return File.Exists(pathToFile);
        }

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
            long folderSize = 0;

            List<string> files = new List<string>(Directory.GetFiles(pathToFolder));

            foreach (string file in files)
            {
                folderSize += file.Length;
            }
            return (folderSize);
        }

        internal bool СheckUniquenessFilename(string pathToFile, string pathToFolder)
        {
            List<string> files = new List<string>(Directory.GetFiles(pathToFolder));
            FileInfo File = new FileInfo(pathToFile);

            for (int i = 0; i < files.Count; i++)
            {
                if (File.Name == (files[i].Remove(0, (pathToFolder.Length + 1))))
                {
                    return false;
                }
            }
            return true;
        }
    }
}


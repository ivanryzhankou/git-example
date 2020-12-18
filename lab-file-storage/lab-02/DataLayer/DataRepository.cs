using System.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace lab_02.DataLayer
{
    class DataRepository
    {

        public void RenameFile(string originalName, string newName)
        {
            File.Move(originalName, newName);
        }

        internal void RemoveFileFromStorage(string pathToFile)
        {
            File.Delete(pathToFile);
        }

        internal void UploadFilesIntoStorage(string pathToFile)
        {
            var fileInf = new FileInfo(pathToFile);
            fileInf.CopyTo((ConfigurationManager.AppSettings.Get("storageAddress") + "\\" + fileInf.Name));
        }

        internal void DownloadFilesFromStorage(string downloadingFile, string pathToDownloadingFile)
        {
            File.Copy(downloadingFile, pathToDownloadingFile, true);
        }

        internal void CreateDirectory (string pathToDirectory)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(pathToDirectory);
            dirInfo.Create();
        }
    }
}


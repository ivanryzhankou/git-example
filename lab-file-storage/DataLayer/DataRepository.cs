using DataLayer.Interfaces;
using System.Configuration;
using System.IO;

namespace DataLayer
{
    public class DataRepository : IDataRepository
    {
        public void RenameFile(string originalName, string newName)
        {
            File.Move(originalName, newName);
        }

        public void RemoveFileFromStorage(string pathToFile)
        {
            File.Delete(pathToFile);
        }

        public void UploadFilesIntoStorage(string pathToFile)
        {
            var fileInf = new FileInfo(pathToFile);
            fileInf.CopyTo((ConfigurationManager.AppSettings.Get("storageAddress") + "\\" + fileInf.Name));
        }

        public void DownloadFilesFromStorage(string downloadingFile, string pathToDownloadingFile)
        {
            File.Copy(downloadingFile, pathToDownloadingFile, true);
        }

        public void CreateDirectory(string pathToDirectory)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(pathToDirectory);
            dirInfo.Create();
        }
    }
}


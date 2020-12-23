namespace DataLayer.Interfaces
{
    public interface IDataRepository
    {
        void RenameFile(string originalName, string newName);
        void RemoveFileFromStorage(string pathToFile);
        void UploadFilesIntoStorage(string pathToFile);
        void DownloadFilesFromStorage(string downloadingFile, string pathToDownloadingFile);
        void CreateDirectory(string pathToDirectory);
    }
}

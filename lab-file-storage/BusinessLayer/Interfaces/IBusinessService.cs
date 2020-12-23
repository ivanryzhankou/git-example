using DataLayer.Models;

namespace BusinessLayer.Interfaces
{
    public interface IBusinessService
	{
		InformationForUser FileUploadCheck(string pathToFile);
		InformationForUser UploadFileIntoStorage(string pathToFile);
		InformationForUser FileDownloadCheck(string downloadingFile, string folderForDownloading);
		InformationForUser DownloadFilesFromStorage(string downloadingFile, string folderFordownloading);
		InformationForUser FileRenameCheck(string oldName, string newName);
		InformationForUser RenameFile(string oldName, string newName);
		FileMetaInformation GetMetainformationAboutFile(string pathToFile);
		InformationForUser RemoveFileFromStorage(string pathToFile);
		bool CheckForInvalidCharacters(string newName);
		string GetFileName(string pathToFile);
		long GetFileStorageSize();
		bool FileSearch(string fileName);
		void SaveCreationDate();
		bool IsBinaryRepositoryExists();
		void CreateBinaryRepository();
		InformationForUser CreateFileStorage(string storageName, string pathToStorage);
	}
}

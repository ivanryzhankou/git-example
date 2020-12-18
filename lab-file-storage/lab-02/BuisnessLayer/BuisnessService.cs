using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using lab_02.DataLayer.Models;
using lab_02.DataLayer;


namespace lab_02.BuisnessLayer
{
    public class BuisnessService
    {
        string storageSddress = ConfigurationManager.AppSettings.Get("storageAddress");

        const long maxFileSize = 157286400; // 150 Megabyte
        const long MaximumStorageSize = 10737418240; // 10 Gigabyte

        private readonly DataRepository _dataRepository;
        private readonly BinaryDataRepository _binaryDataRepository;
        private readonly ConfigurationDataRepository _configurationDataRepository;

        private InformationForUser _userInformation;

        public BuisnessService()
        {
            _dataRepository = new DataRepository();
            _binaryDataRepository = new BinaryDataRepository();
            _configurationDataRepository = new ConfigurationDataRepository();
            _userInformation = new InformationForUser();
        }

        public InformationForUser FileUploadCheck(string pathToFile)
        {

            if (!_IsFileExistence(pathToFile))
            {
                _userInformation.informationForUser = "This file does not exist. Please try again";
                return _userInformation;
            }

            if (_CheckOnMaxSizeFile(pathToFile))
            {
                _userInformation.informationForUser = "Sorry.The file cannot be larger than 150 MB";
                return _userInformation;
            }

            if (_CheckStorageOverflow(pathToFile))
            {
                _userInformation.informationForUser = "Sorry.You cannot store more than 10 gigabytes.Pay for an increase in available storage or select a different file";
                return _userInformation;
            }

            if (!_IsFileNameUnique(pathToFile, storageSddress))
            {
                _userInformation.informationForUser = "A file with the same name already exists";
                return _userInformation;
            }

            _userInformation = UploadFileIntoStorage(pathToFile);
            return _userInformation;
        }

        internal InformationForUser UploadFileIntoStorage(string pathToFile)
        {
            _dataRepository.UploadFilesIntoStorage(pathToFile);

            if (_IsFileExistence(pathToFile, storageSddress))
            {
                AddNewFileMetoinformation(pathToFile);

                _userInformation.informationForUser = "The file has been successfully uploaded to the storage. Press any key to return to the menu";

                return _userInformation;
            }
            _userInformation.informationForUser = "File was not uploadeded. try again. Press any key to return to the menu";

            return _userInformation;
        }

        internal InformationForUser FileDownloadCheck(string downloadingFile, string folderForDownloading)
        {
            if (!Directory.Exists(folderForDownloading))
            {
                _userInformation.informationForUser = "This directory does not exist. Please try again";

                return _userInformation;
            }

            if (!_IsFileNameUnique(downloadingFile, folderForDownloading))
            {
                _userInformation.informationForUser = "A file with the same name already exists. Please try again";

                return _userInformation;
            }

            _userInformation = DownloadFilesFromStorage(downloadingFile, folderForDownloading);

            return _userInformation;
        }

        internal InformationForUser DownloadFilesFromStorage(string downloadingFile, string folderFordownloading)
        {
            string pathTodownloadingFile = $"{folderFordownloading}\\{GetFileName(downloadingFile)}";

            _dataRepository.DownloadFilesFromStorage(downloadingFile, pathTodownloadingFile);

            _userInformation.isOperationValid = _IsFileExistence(downloadingFile, folderFordownloading);

            if (_userInformation.isOperationValid)
            {
                IncrementCountOfDownloads(downloadingFile);

                _userInformation.informationForUser = "The file has been successfully download from storage. Press any key to return to the menu";

                return _userInformation;
            }
            _userInformation.informationForUser = "File was not download. Try again. Press any key to return to the menu";

            return _userInformation;
        }

        public InformationForUser FileRenameCheck(string oldName, string newName)
        {
            if (_IsFileExistence($"{storageSddress}\\{newName}"))
            {
                _userInformation.informationForUser = "File with the same name already exists. Try again";
                return _userInformation;
            }

            if (oldName == newName)
            {
                _userInformation.informationForUser = "You haven't changed the file name. Try again";
                return _userInformation;
            }

            if (!CheckForInvalidCharacters(newName))
            {
                _userInformation.informationForUser = @"the file name cannot be used '/\:*?«<>|' try again";
                return _userInformation;
            }

            if (newName.Length > 250)
            {
                _userInformation.informationForUser = "The file name cannot exceed 250 characters . Try again";
                return _userInformation;
            }

            _userInformation = RenameFile(oldName, newName);
            return _userInformation;
        }

        public InformationForUser RenameFile(string oldName, string newName)
        {
            string pathToRenamedFile = $"{storageSddress}\\{newName}";

            _dataRepository.RenameFile(oldName, pathToRenamedFile);

            if (_IsFileExistence(pathToRenamedFile))
            {
                RenameFileMetoinformation(oldName, newName);

                _userInformation.informationForUser = "File has been renamed. Press any key to return to the menu";
                return _userInformation;
            }

            _userInformation.informationForUser = "File cannot be renamed. Try again. Press any key to return to the menu";
            return _userInformation;
        }

        internal FileMetaInformation GetMetainformationAboutFile(string pathToFile)
        {
            string fileName = GetFileName(pathToFile);

            Dictionary<string, FileMetaInformation> metaInformationFiles = _binaryDataRepository.DeserializeFileMetaInformation();
            FileMetaInformation informationAboutselectedFile = metaInformationFiles.GetValueOrDefault(fileName);

            return informationAboutselectedFile;
        }

        internal InformationForUser RemoveFileFromStorage(string pathToFile)
        {
            _dataRepository.RemoveFileFromStorage(pathToFile);

            if (!_IsFileExistence(pathToFile))
            {
                RemoveFileMetoinformation(pathToFile);

                _userInformation.informationForUser = "File has been delete. Press any key to return to the menu";
                return _userInformation;
            }
            else
            {
                _userInformation.informationForUser = "File cannot be delete. Try again. Press any key to return to the menu";
                return _userInformation;
            }
        }

        public bool CheckForInvalidCharacters(string newName)
        {
            return !(newName.IndexOfAny("/\\:*?«<>|".ToCharArray()) >= 0);
        }

        private void AddNewFileMetoinformation(string pathToFile) // binD?
        {
            var fileMetaInformation = GetMetaInformationAboutFile(pathToFile);
            Dictionary<string, FileMetaInformation> metaInformationFiles = _binaryDataRepository.DeserializeFileMetaInformation();

            metaInformationFiles.Add(fileMetaInformation.name, fileMetaInformation);

            _binaryDataRepository.SerializeFileMetaInformation(metaInformationFiles);
        }

        private void IncrementCountOfDownloads(string pathToFile)
        {
            string fileName = GetFileName(pathToFile);
            Dictionary<string, FileMetaInformation> metaInformationFiles = _binaryDataRepository.DeserializeFileMetaInformation();
            FileMetaInformation informationAboutselectedFile = metaInformationFiles.GetValueOrDefault(fileName);

            informationAboutselectedFile.downloadСounter++;
            metaInformationFiles[fileName] = informationAboutselectedFile;

            _binaryDataRepository.SerializeFileMetaInformation(metaInformationFiles);
        }

        private void RenameFileMetoinformation(string pathToFile, string newName)
        {
            string oldName = GetFileName(pathToFile);

            Dictionary<string, FileMetaInformation> metaInformationFiles = _binaryDataRepository.DeserializeFileMetaInformation();
            FileMetaInformation informationAboutselectedFile = metaInformationFiles.GetValueOrDefault(oldName);

            informationAboutselectedFile.name = newName;
            metaInformationFiles[newName] = informationAboutselectedFile;
            metaInformationFiles.Remove(oldName);

            _binaryDataRepository.SerializeFileMetaInformation(metaInformationFiles);

        }

        private void RemoveFileMetoinformation(string pathToFile)
        {
            string fileName = GetFileName(pathToFile);
            Dictionary<string, FileMetaInformation> metaInformationFiles = _binaryDataRepository.DeserializeFileMetaInformation();

            metaInformationFiles.Remove(fileName);

            _binaryDataRepository.SerializeFileMetaInformation(metaInformationFiles);
        }

        private FileMetaInformation GetMetaInformationAboutFile(string pathToFile)
        {
            FileMetaInformation fileMetaInformation = new FileMetaInformation();

            fileMetaInformation.name = GetFileName(pathToFile);
            fileMetaInformation.extension = Path.GetExtension(pathToFile);
            fileMetaInformation.size = _GetFileSize(pathToFile);
            fileMetaInformation.creationDate = DateTime.Now.ToString("yyyy-MM-dd");
            fileMetaInformation.downloadСounter = 0;
            fileMetaInformation.hashChecksum = GetHashChecksum(pathToFile);

            return fileMetaInformation;
        }

        private string GetHashChecksum(string pathToFile)
        {
            string fileHash = string.Empty;

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(pathToFile))
                {
                    var hash = (md5.ComputeHash(stream));

                    for (int i = 0; i < hash.Length; i++)
                    {
                        fileHash += hash[i];
                    }
                }
            }
            return fileHash;
        }

        internal string GetFileName(string pathToFile)
        {
            string fileName = string.Empty;

            for (int i = pathToFile.Length - 1; i >= 0; i--)
            {
                if (pathToFile[i] != '\\')
                {
                    fileName += pathToFile[i];
                }
                else
                {
                    break;
                }
            }
            string fileNameReverse = new string(fileName.ToCharArray().Reverse().ToArray());

            return fileNameReverse;
        }

        internal long GetFileStorageSize()
        {
            return _GetFolderSize(storageSddress);
        }
        internal bool FileSearch(string fileName)
        {
            HashSet<string> files = new HashSet<string>(Directory.GetFiles(ConfigurationManager.AppSettings.Get("storageAddress")));

            return files.Contains(fileName);
        }

        private bool _IsFileExistence(string pathToFile)
        {
            return File.Exists(pathToFile);
        }

        private bool _IsFileExistence(string psthTofileName, string pathToFolder)
        {
            string fileName = GetFileName(psthTofileName);
            string pathToFile = $"{pathToFolder}\\{fileName}";

            return File.Exists(pathToFile);
        }

        internal void SaveCreationDate()
        {
            _configurationDataRepository.AddUpdateAppSettings("creationDate", DateTime.Now.ToString("yyyy-MM-dd"));
        }

        internal bool IsBinaryRepositoryExists()
        {
            string pathToBinaryRepository = ConfigurationManager.AppSettings.Get("storageAddress");

            return _IsFileExistence(pathToBinaryRepository);
        }

        internal void CreateBinaryRepository()
        {
            Dictionary<string, FileMetaInformation> metaInformationFiles = new Dictionary<string, FileMetaInformation>();

            _binaryDataRepository.SerializeFileMetaInformation(metaInformationFiles);
        }

        internal InformationForUser CreateFileStorage(string storageName, string pathToStorage)
        {
            if (!Directory.Exists(pathToStorage))
            {
                _userInformation.informationForUser = "Specified directory does not exist. Try again";
                return _userInformation;
            }

            if (!_СheckUniquenessFolderName(storageName, pathToStorage))
            {
                _userInformation.informationForUser = "Folder with the same name already exists at the specified address. Try again";
                return _userInformation;
            }

            _configurationDataRepository.AddUpdateAppSettings("storageAddress", storageName);
            SaveCreationDate();
            _dataRepository.CreateDirectory(storageName);

            _userInformation.informationForUser = $"file store was created along the path: {storageName}. Re-enter the program to get started";
            _userInformation.isOperationValid = true;
            return _userInformation;
        }

        private bool _CheckStorageOverflow(string pathToFile) //b
        {
            return _GetFolderSize(ConfigurationManager.AppSettings.Get("storageAddress")) + _GetFileSize(pathToFile) > MaximumStorageSize;
        }

        private long _GetFileSize(string pathToFile) //b
        {
            FileInfo File = new FileInfo(pathToFile);

            return File.Length;
        }

        private bool _CheckOnMaxSizeFile(string pathToFile)
        {
            return _GetFileSize(pathToFile) > maxFileSize;
        }

        private long _GetFolderSize(string pathToFolder)
        {
            List<string> files = new List<string>(Directory.GetFiles(pathToFolder));

            return files.Select(x => x.Length).Sum();
        }

        private bool _IsFileNameUnique(string pathToFile, string pathToFolder)
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

        private bool _СheckUniquenessFolderName(string storageName, string pathToFolder)
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
    }
}

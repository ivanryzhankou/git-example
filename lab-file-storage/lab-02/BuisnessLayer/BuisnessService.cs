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


        private readonly DataRepository _dataRepository;
        private readonly BinaryDataRepository _binaryDataRepository;
        private readonly ConfigurationDataRepository _configurationDataRepository;

        private InformationForUser _userInformation;

        public BuisnessService()
        {
            _dataRepository = new DataRepository();
            _binaryDataRepository = new BinaryDataRepository();
            _configurationDataRepository = new ConfigurationDataRepository ();
            _userInformation = new InformationForUser();
        }

        public InformationForUser CheckingRenameFile(string oldName, string newName)
        {
            if (_dataRepository.IsFileExistence($"{storageSddress}\\{newName}"))
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

            _userInformation.isOperationValid = true;
            return _userInformation;

        }

        internal FileMetaInformation GetInformationAboutFile(string pathToFile)
        {
            string fileName = GetFileName(pathToFile);

            Dictionary<string, FileMetaInformation> metaInformationFiles = _binaryDataRepository.DeserializeFileMetaInformation();
            FileMetaInformation informationAboutselectedFile = metaInformationFiles.GetValueOrDefault(fileName);

            return informationAboutselectedFile;


        }

        internal InformationForUser RemoveFileFromStorage(string pathToFile)
        {
            _dataRepository.DeleteFileFromStorage(pathToFile);

            if (!_dataRepository.IsFileExistence(pathToFile))
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

        public InformationForUser RenameFile(string oldName, string newName)
        {
            _dataRepository.RenameFile(oldName, $"{storageSddress}\\{newName}");

            if (_dataRepository.IsFileExistence($"{storageSddress}\\{newName}"))
            {
                RenameFileMetoinformation(oldName, newName);

                _userInformation.informationForUser = "File has been renamed. Press any key to return to the menu";
                return _userInformation;
            }

            _userInformation.informationForUser = "File cannot be renamed. Try again. Press any key to return to the menu";
            return _userInformation;

        }

        public bool CheckForInvalidCharacters(string newName)
        {
            List<char> invalidCharacters = new List<char>() { '/', '\\', ':', '*', '?', '«', '<', '>', '|' };

            return !(newName.IndexOfAny("/\\:*?«<>|".ToCharArray()) >= 0);
        }

        public InformationForUser CheckFileUpload(string pathToFile)
        {

            if (!_dataRepository.IsFileExistence(pathToFile))
            {
                _userInformation.informationForUser = "This file does not exist. Please try again";
                return _userInformation;
            }

            if (_dataRepository.CheckOnMaxSizeFile(pathToFile))
            {
                _userInformation.informationForUser = "Sorry.The file cannot be larger than 150 MB";
                return _userInformation;
            }

            if (_dataRepository.CheckStorageOverflow(pathToFile))
            {
                _userInformation.informationForUser = "Sorry.You cannot store more than 10 gigabytes.Pay for an increase in available storage or select a different file";
                return _userInformation;
            }

            if (!_dataRepository.IsFileNameUnique(pathToFile, storageSddress))
            {
                _userInformation.informationForUser = "A file with the same name already exists";
                return _userInformation;
            }

            else
            {
                _userInformation.isOperationValid = true;
                return _userInformation;
            }
        }

        internal InformationForUser UploadFileIntoStorage(string pathToFile)
        {
            _dataRepository.UploadFilesIntoStorage(pathToFile);

            if (CheckOnUploadSuccess(pathToFile, storageSddress))
            {
                AddNewFileMetoinformation(pathToFile);

                _userInformation.informationForUser = "The file has been successfully uploaded to the storage. Press any key to return to the menu";

                return _userInformation;
            }
            else
            {
                _userInformation.informationForUser = "File was not uploadeded. try again. Press any key to return to the menu";

                return _userInformation;
            }
        }

        private void AddNewFileMetoinformation(string pathToFile)
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
            fileMetaInformation.size = _dataRepository.GetFileSize(pathToFile);
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

        internal InformationForUser DownloadFilesIntoStorage(string unloadingFile, string folderForUnloading)
        {
            string pathToUnloadingFile = $"{folderForUnloading}\\{GetFileName(unloadingFile)}";

            _dataRepository.DownloadFilesIntoStorage(unloadingFile, pathToUnloadingFile);

            _userInformation.isOperationValid = CheckOnUploadSuccess(unloadingFile, pathToUnloadingFile);

            if (_userInformation.isOperationValid)
            {
                IncrementCountOfDownloads(unloadingFile);

                _userInformation.informationForUser = "The file has been successfully uploaded to the storage. Press any key to return to the menu";

                return _userInformation;
            }
            _userInformation.informationForUser = "File was not uploadeded. Try again. Press any key to return to the menu";

            return _userInformation;
        }

        internal InformationForUser CheckFileForUnload(string unloadingFile, string folderForUnloading)
        {
            if (!Directory.Exists(folderForUnloading))
            {
                _userInformation.informationForUser = "This directory does not exist. Please try again";

                return _userInformation;
            }

            if (!_dataRepository.IsFileNameUnique(unloadingFile, folderForUnloading))
            {
                _userInformation.informationForUser = "A file with the same name already exists at the given path. Replace it?";
                _userInformation.needReplacement = true;

                return _userInformation;
            }

            _userInformation.isOperationValid = true;
            return _userInformation;

        }

        private bool CheckOnUploadSuccess(string pathToFile, string pathToFolder)
        {
            string fileName = GetFileName(pathToFile);

            return _dataRepository.IsFileExistence(storageSddress + "//" + fileName);
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
            return _dataRepository.GetFolderSize(storageSddress);
        }
        internal bool FileSearch(string fileName)
        {
            HashSet<string> files = new HashSet<string>(Directory.GetFiles(ConfigurationManager.AppSettings.Get("storageAddress")));

            return files.Contains(fileName);

        }

        internal void SaveCreationDate()
        {
            _configurationDataRepository.AddUpdateAppSettings("creationDate", DateTime.Now.ToString("yyyy-MM-dd"));
        }

        internal bool IsBinaryRepositoryExists()
        {
            string pathToBinaryRepository = ConfigurationManager.AppSettings.Get("storageAddress");

            return _dataRepository.IsFileExistence(pathToBinaryRepository);
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

            if (!_dataRepository.СheckUniquenessFolderName(storageName, pathToStorage))
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
    }
}

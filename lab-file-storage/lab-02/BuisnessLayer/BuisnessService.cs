using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using lab_02.BuisnessLayer.Model;
using lab_02.DataLayer.Models;


namespace lab_02.BuisnessLayer
{
    public class BuisnessService
    {
        string storageSddress = ConfigurationManager.AppSettings.Get("storageAddress");

        DataLayer.DataRepository dataRepository = new DataLayer.DataRepository();
        DataLayer.BinaryDataRepository binaryDataRepository = new DataLayer.BinaryDataRepository();
        DataLayer.ConfigurationDataRepository configurationDataRepository = new DataLayer.ConfigurationDataRepository();

        InformationForUser userInformation = new InformationForUser();

        public InformationForUser CheckingRenameFile(string oldName, string newName)
        {
            if (dataRepository.IsFileExistence(storageSddress + "\\" + newName))
            {
                userInformation.informationForUser = "File with the same name already exists. Try again";
                return userInformation;
            }

            if (oldName == newName)
            {
                userInformation.informationForUser = "You haven't changed the file name. Try again";
                return userInformation;
            }

            if (!CheckForInvalidCharacters(newName))
            {
                userInformation.informationForUser = @"the file name cannot be used '/\:*?«<>|' try again";
                return userInformation;
            }

            if (newName.Length > 250)
            {
                userInformation.informationForUser = "The file name cannot exceed 250 characters . Try again";
                return userInformation;
            }
            //todo: useless else statement
            else
            {
                userInformation.isOperationValid = true;
                return userInformation;
            }
        }

        internal FileMetaInformation GetInformationAboutFile (string pathToFile)
        {
            string fileName = GetFileName(pathToFile);

            Dictionary<string, FileMetaInformation> metaInformationFiles = binaryDataRepository.DeserializeFileMetaInformation();
            FileMetaInformation informationAboutselectedFile = metaInformationFiles.GetValueOrDefault(fileName);

            return informationAboutselectedFile;


        }

        internal InformationForUser RemoveFileFromStorage(string pathToFile)
        {
            dataRepository.DeleteFileFromStorage(pathToFile);

            if (!dataRepository.IsFileExistence(pathToFile))
            {
                RemoveFileMetoinformation(pathToFile);

                userInformation.informationForUser = "File has been delete. Press any key to return to the menu";
                return userInformation;
            }
            else
            {
                userInformation.informationForUser = "File cannot be delete. Try again. Press any key to return to the menu";
                return userInformation;
            }
        }

        public InformationForUser RenameFile(string oldName, string newName)
        {
            dataRepository.RenameFile(oldName, storageSddress + "\\" + newName);

            if (dataRepository.IsFileExistence(storageSddress + "\\" + newName))
            {
                RenameFileMetoinformation(oldName, newName);

                userInformation.informationForUser = "File has been renamed. Press any key to return to the menu";
                return userInformation;
            }
            //todo: useless else statement
            else
            {
                userInformation.informationForUser = "File cannot be renamed. Try again. Press any key to return to the menu";
                return userInformation;
            }
        }

        public bool CheckForInvalidCharacters(string newName)
        {
           List<char> invalidCharacters = new List<char>() {'/', '\\', ':', '*', '?', '«', '<', '>', '|' };
            //todo: the whole can be simplified smth like this: newName.IndexOfAny("/*?".ToCharArray()) >= 0  
            for (int i = 0; i < newName.Length; i++)
            {
                for (int j = 0; j < invalidCharacters.Count; j++)
                {
                    if (newName[i] == invalidCharacters[j])
                    {
                        return false;
                    }
            }
        }
            return true;
        }

    public InformationForUser CheckingFileUpload(string pathToFile)
        {

            if (!dataRepository.IsFileExistence(pathToFile))
            {
                userInformation.informationForUser = "This file does not exist. Please try again";
                return userInformation;
            }

            if (dataRepository.CheckOnMaxSizeFile(pathToFile))
            {
                userInformation.informationForUser = "Sorry.The file cannot be larger than 150 MB";
                return userInformation;
            }

            if (dataRepository.checkOnStorageOverflow(pathToFile))
            {
                userInformation.informationForUser = "Sorry.You cannot store more than 10 gigabytes.Pay for an increase in available storage or select a different file";
                return userInformation;
            }

            if (!dataRepository.СheckUniquenessFilename(pathToFile, storageSddress))
            {
                userInformation.informationForUser = "A file with the same name already exists";
                return userInformation;
            }

            else
            {
                userInformation.isOperationValid = true;
                return userInformation;
            }
        }

        internal InformationForUser UploadFileIntoStorage(string pathToFile)
        {
            dataRepository.UploadFilesIntoStorage(pathToFile);

            if (CheckOnUploadSuccess(pathToFile, storageSddress))
            {
                AddNewFileMetoinformation(pathToFile);

                userInformation.informationForUser = "The file has been successfully uploaded to the storage. Press any key to return to the menu";

                return userInformation;
            }
            else
            {
                userInformation.informationForUser = "File was not uploadeded. try again. Press any key to return to the menu";

                return userInformation;
            }
        }

        private void AddNewFileMetoinformation (string pathToFile)
        {
            var fileMetaInformation = GetMetaInformationAboutFile(pathToFile);
            Dictionary<string, FileMetaInformation> metaInformationFiles = binaryDataRepository.DeserializeFileMetaInformation();

            metaInformationFiles.Add(fileMetaInformation.name, fileMetaInformation);

            binaryDataRepository.SerializeFileMetaInformation(metaInformationFiles);
        }

        private void IncrementCountOfDownloads (string pathToFile)
        {
            string fileName = GetFileName(pathToFile);
            Dictionary<string, FileMetaInformation> metaInformationFiles = binaryDataRepository.DeserializeFileMetaInformation();
            FileMetaInformation informationAboutselectedFile = metaInformationFiles.GetValueOrDefault(fileName);

            informationAboutselectedFile.downloadСounter++;
            metaInformationFiles[fileName] = informationAboutselectedFile;

            binaryDataRepository.SerializeFileMetaInformation(metaInformationFiles);
        }

        private void RenameFileMetoinformation(string pathToFile, string newName)   // need optimization
        {
            string oldName = GetFileName(pathToFile);

            Dictionary<string, FileMetaInformation> metaInformationFiles = binaryDataRepository.DeserializeFileMetaInformation();
            FileMetaInformation informationAboutselectedFile = metaInformationFiles.GetValueOrDefault(oldName);

            informationAboutselectedFile.name = newName;
            metaInformationFiles[newName] = informationAboutselectedFile;
            metaInformationFiles.Remove(oldName);

            binaryDataRepository.SerializeFileMetaInformation(metaInformationFiles);

        }

        private void RemoveFileMetoinformation(string pathToFile)
        {
            string fileName = GetFileName(pathToFile);
            Dictionary<string, FileMetaInformation> metaInformationFiles = binaryDataRepository.DeserializeFileMetaInformation();

            metaInformationFiles.Remove(fileName);

            binaryDataRepository.SerializeFileMetaInformation(metaInformationFiles);
        }

        private FileMetaInformation GetMetaInformationAboutFile(string pathToFile)
        {
            FileMetaInformation fileMetaInformation = new FileMetaInformation();

            fileMetaInformation.name = GetFileName(pathToFile);
            fileMetaInformation.extension = Path.GetExtension(pathToFile);
            fileMetaInformation.size = dataRepository.GetFileSize(pathToFile);
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

        internal InformationForUser UnloadFilesIntoStorage(string unloadingFile, string folderForUnloading)
        {
            string pathToUnloadingFile = folderForUnloading + "\\" + GetFileName(unloadingFile);

            dataRepository.UnloadFilesIntoStorge(unloadingFile, pathToUnloadingFile);

            userInformation.isOperationValid = CheckOnUploadSuccess(unloadingFile, pathToUnloadingFile);

            if (userInformation.isOperationValid)
            {
                IncrementCountOfDownloads(unloadingFile);

                userInformation.informationForUser = "The file has been successfully uploaded to the storage. Press any key to return to the menu";

                return userInformation;
            }
            //todo: useless else statement
            else
            {
                userInformation.informationForUser = "File was not uploadeded. Try again. Press any key to return to the menu";

                return userInformation;
            }
        }

        internal InformationForUser CheckFileForUnload(string unloadingFile, string folderForUnloading)
        {
            if (!Directory.Exists(folderForUnloading))
            {
                userInformation.informationForUser = "This directory does not exist. Please try again";

                return userInformation;
            }

            if (!dataRepository.СheckUniquenessFilename(unloadingFile, folderForUnloading))
            {
                userInformation.informationForUser = "A file with the same name already exists at the given path. Replace it?";
                userInformation.needReplacement = true;

                return userInformation;
            }
            //todo: useless else statement
            else
            {
                userInformation.isOperationValid = true;
                return userInformation;
            }
        }

        private bool CheckOnUploadSuccess(string pathToFile, string pathToFolder)
        {
            string fileName = GetFileName(pathToFile);

            return dataRepository.IsFileExistence(storageSddress + "//" + fileName);
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
            //todo: haven't seen creating string thru new 
            string fileNameReverse = new string(fileName.ToCharArray().Reverse().ToArray());

            return fileNameReverse;
        }

        internal long GetFileStorageSize ()
        {
            return dataRepository.GetFolderSize(storageSddress);
        }
        //todo: 
        internal bool FileSearch(string fileName)
        {
            HashSet<string> files = new HashSet<string>(Directory.GetFiles(ConfigurationManager.AppSettings.Get("storageAddress"))); ;


            //todo: can be replaced with return files.Contains(fileName);
            if (files.Contains(fileName))
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        internal void SaveCreationDate ()
        {
            configurationDataRepository.AddUpdateAppSettings("creationDate", DateTime.Now.ToString("yyyy-MM-dd"));
        }

        //todo: no need to add space after method name CheckForExistenceOfBinaryRepository (), just CheckForExistenceOfBinaryRepository()
        //todo: I'd name it like IsBinaryRepositoryExists
        internal bool CheckForExistenceOfBinaryRepository ()
        {
            string pathToBinaryRepository = ConfigurationManager.AppSettings.Get("storageAddress");

            return dataRepository.IsFileExistence(pathToBinaryRepository);
        }

        internal void CreateBinaryRepository()
        {
            Dictionary<string, FileMetaInformation> metaInformationFiles = new Dictionary<string, FileMetaInformation>();

            binaryDataRepository.SerializeFileMetaInformation(metaInformationFiles);
        }

        internal InformationForUser CreateFileStorage(string storageName, string pathToStorage)
        {
            if (!Directory.Exists(pathToStorage))
            {
                userInformation.informationForUser = "Specified directory does not exist. Try again";
                return userInformation;
            }

            if (!dataRepository.СheckUniquenessFolderName(storageName, pathToStorage))
            {
                userInformation.informationForUser = "Folder with the same name already exists at the specified address. Try again";
                return userInformation;
            }
            //todo: you don't need this else
            else
            {
                configurationDataRepository.AddUpdateAppSettings("storageAddress", storageName);
                SaveCreationDate();
                dataRepository.CreateDirectory(storageName);

                userInformation.informationForUser = $"file store was created along the path: {storageName}. Re-enter the program to get started";
                userInformation.isOperationValid = true;
                return userInformation;
            }
        }
    }
}

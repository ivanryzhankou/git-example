using System;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
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

            else
            {
                userInformation.isFileValid = true;
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

        internal InformationForUser DeleteFileFromStorage(string pathToFile)
        {
            dataRepository.DeleteFileFromStorage(pathToFile);

            if (!dataRepository.IsFileExistence(pathToFile))
            {
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
                userInformation.informationForUser = "File has been renamed. Press any key to return to the menu";
                return userInformation;
            }
            else
            {
                userInformation.informationForUser = "File cannot be renamed. Try again. Press any key to return to the menu";
                return userInformation;
            }
        }

        public bool CheckForInvalidCharacters(string newName)
        {
           List<char> invalidCharacters = new List<char>() {'/', '\\', ':', '*', '?', '«', '<', '>', '|' };

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
                userInformation.isFileValid = true;
                return userInformation;
            }
        }

        internal string UploadFileIntoStorage(string pathToFile)
        {
            dataRepository.UploadFilesIntoStorage(pathToFile);

            if (CheckOnUploadSuccess(pathToFile, storageSddress))
            {
                SaveNewFileMetoinformation(pathToFile);

                return "The file has been successfully uploaded to the storage. Press any key to return to the menu";
            }
            else
            {
                userInformation.informationForUser = "File was not uploadeded. try again. Press any key to return to the menu";

                return "File was not uploadeded. try again";
            }
        }

        private void SaveNewFileMetoinformation (string pathToFile)
        {
            var fileMetaInformation = GetMetaInformationAboutFile(pathToFile);
            binaryDataRepository.SerializeFileMetaInformation(fileMetaInformation);
        }

        private FileMetaInformation GetMetaInformationAboutFile(string pathToFile)
        {
            FileMetaInformation fileMetaInformation = new FileMetaInformation();

            fileMetaInformation.name = GetFileName(pathToFile);
            fileMetaInformation.extension = Path.GetExtension(pathToFile);
            fileMetaInformation.size = dataRepository.GetFileSize(pathToFile);
            fileMetaInformation.creationDate = DateTime.Now.ToString("yyyy-MM-dd");
            fileMetaInformation.downloadsNumber = 0;
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

        internal string UnloadFilesIntoStorage(string unloadingFile, string folderForUnloading)
        {
            string pathToUnloadingFile = folderForUnloading + "\\" + GetFileName(unloadingFile);

            dataRepository.UnloadFilesIntoStorge(unloadingFile, pathToUnloadingFile);

            userInformation.isFileValid = CheckOnUploadSuccess(unloadingFile, pathToUnloadingFile);

            if (userInformation.isFileValid)
            {
                return "The file has been successfully uploaded to the storage. Press any key to return to the menu";
            }

            else
            {
                return "File was not uploadeded. Try again. Press any key to return to the menu";
            }
        }

        internal InformationForUser CheckFileForUnload(string unloadingFile, string folderForUnloading)
        {
            InformationForUser userInformation = new InformationForUser();

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

            else
            {
                userInformation.isFileValid = true;
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
            string fileNameReverse = new string(fileName.ToCharArray().Reverse().ToArray());

            return fileNameReverse;
        }
    }
}

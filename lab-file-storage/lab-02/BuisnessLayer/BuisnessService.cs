using System;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using lab_02.BuisnessLayer.Model;

namespace lab_02.BuisnessLayer
{
    public class BuisnessService
    {
        string storageSddress = ConfigurationManager.AppSettings.Get("storageAddress");

        DataLayer.DataRepository dataRepository = new DataLayer.DataRepository();
        DataLayer.BinaryDataRepository binaryDataRepository = new DataLayer.BinaryDataRepository();

        UserInformation userInformation = new UserInformation();


        public UserInformation CheckingFileUpload(string pathToFile)
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
                UploadFilesIntoStorage(pathToFile);

                bool downloadSuccessful = CheckOnUploadSuccess(pathToFile, storageSddress);

                if (downloadSuccessful == true)
                {

                    GetMetaInformationAboutFile(pathToFile);
                    userInformation.informationForUser = "The file has been successfully uploaded to the storage. Press enter to return to the menu";
                    return userInformation;
                }
                else
                {
                    userInformation.informationForUser = "File was not uploadeded. try again";
                    return userInformation;
                }
            }
        }

        private void GetMetaInformationAboutFile(string pathToFile)
        {
            string name = dataRepository.GetFileName(pathToFile);
            string extension = Path.GetExtension(pathToFile);
            long size = dataRepository.GetFileSize(pathToFile);
            string creationDate = DateTime.Now.ToString("yyyy-MM-dd");
            int downloadsNumber = 0;
            string hashChecksum = GetHashChecksum(pathToFile);

            binaryDataRepository.SerializeFileMetaInformation(name, extension, size, creationDate, downloadsNumber, hashChecksum);
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
        private bool UploadFilesIntoStorage(string pathToFile)
        {
            dataRepository.UploadFilesIntoStorage(pathToFile);

            return CheckOnUploadSuccess(pathToFile, storageSddress);
        }
       
        internal UserInformation UnloadFilesIntoStorage(string unloadingFile, string folderForUnloading)
        {
            UserInformation userInformation = new UserInformation();

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
                dataRepository.UnloadFilesIntoStorge(unloadingFile, folderForUnloading);

                userInformation.isFileValid = CheckOnUploadSuccess(unloadingFile, folderForUnloading);

                if (userInformation.isFileValid)
                {
                    userInformation.informationForUser = "The file has been successfully uploaded to the storage. Press enter to return to the menu";

                }

                else
                {
                    userInformation.informationForUser = "File was not uploadeded. try again";
                }

                return userInformation;
            }
        }

        private void CheckFileForUnload ()
        {


        }


        private bool CheckOnUploadSuccess(string pathToFile, string pathToFolder)
        {
            string fileName = dataRepository.GetFileName(pathToFile);

            return dataRepository.IsFileExistence(storageSddress + "//" + fileName);
        }

        internal void CheckingRenameResult(string oldName, string newName)
        {

        }
    }
}

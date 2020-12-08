using System;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;

namespace lab_02.BuisnessLayer
{
    class BuisnessService
    {
        string storageSddress = ConfigurationManager.AppSettings.Get("storageAddress");

        DataLayer.DataRepository dataRepository = new DataLayer.DataRepository();
        DataLayer.BinaryDataRepository BinaryDataRepository = new DataLayer.BinaryDataRepository();

        internal (bool, string) FileUploadCheck(string pathToFile)
        {
            var resultOfChecking = (isFileValid: true, downloadResultMessage: string.Empty);

            if (!dataRepository.IsFileExistence(pathToFile))
            {
                return (resultOfChecking.isFileValid = false, resultOfChecking.downloadResultMessage = "This file does not exist. Please try again");
            }

            if (dataRepository.CheckOnMaxSizeFile(pathToFile))
            {
                return (resultOfChecking.isFileValid = false, resultOfChecking.downloadResultMessage = "Sorry.The file cannot be larger than 150 MB");
            }

            if (dataRepository.checkOnStorageOverflow(pathToFile))
            {
                return (resultOfChecking.isFileValid = false, resultOfChecking.downloadResultMessage = "Sorry.You cannot store more than 10 gigabytes.Pay for an increase in available storage or select a different file");
            }

            if (!dataRepository.СheckUniquenessFilename(pathToFile, storageSddress))
            {
                return (resultOfChecking.isFileValid = false, resultOfChecking.downloadResultMessage = "A file with the same name already exists");
            }

            else
            {
                UploadFilesIntoStorage(pathToFile);

                bool downloadSuccessful = CheckOnUploadSuccesa(pathToFile, storageSddress);

                if (downloadSuccessful == true)
                {

                    GetMetaInformationAboutFile(pathToFile);
                    return (resultOfChecking.isFileValid = true, "The file has been successfully uploaded to the storage. Press inter to return to the menu");
                }
                else
                {
                    return (resultOfChecking.isFileValid = false, "File was not uploadeded. try again");
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

            BinaryDataRepository.SerializeFileMetaInformation(name, extension, size, creationDate, downloadsNumber, hashChecksum);
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

            return CheckOnUploadSuccesa(pathToFile, storageSddress);
        }

        private bool CheckOnUploadSuccesa(string pathToFile, string pathToFolder)
        {
            string fileName = dataRepository.GetFileName(pathToFile);

            return dataRepository.IsFileExistence(storageSddress + "//" + fileName);
        }

        internal (bool, bool, string) FileUnloadCheck(string unloadingFile, string folderForUnloading)
        {
            var resultOfChecking = (isFileValid: true, fileNeedReplacment: false, downloadResultMessage: string.Empty);


            if (!Directory.Exists(folderForUnloading))
            {
                return (resultOfChecking.isFileValid = false, resultOfChecking.fileNeedReplacment, resultOfChecking.downloadResultMessage = 
                    "This directory does not exist. Please try again");

            }

            if (!dataRepository.СheckUniquenessFilename(unloadingFile, folderForUnloading))
            {
                return (resultOfChecking.isFileValid = false, resultOfChecking.fileNeedReplacment = true, resultOfChecking.downloadResultMessage = 
                    "A file with the same name already exists at the given path. Replace it?");
            }
            else
            {
                UnloadFilesIntoStorage(unloadingFile, folderForUnloading);

                bool UnloadSuccessful = CheckOnUploadSuccesa(unloadingFile, folderForUnloading);

                if (UnloadSuccessful)
                {
                    return (resultOfChecking.isFileValid = true, resultOfChecking.fileNeedReplacment = true, "The file has been successfully uploaded to the storage. Press inter to return to the menu");

                }
                else
                {
                    return (resultOfChecking.isFileValid = false, resultOfChecking.fileNeedReplacment = true, "File was not uploadeded. try again");
                }
            }
        }

        private bool UnloadFilesIntoStorage(string unloadingFile, string folderForUnloading)
        {
            dataRepository.UnloadFilesIntoStorge(unloadingFile, folderForUnloading);

            return CheckOnUploadSuccesa(unloadingFile, folderForUnloading);
        }
    }
}

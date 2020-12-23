using BusinessLayer.Interfaces;
using DataLayer.Models;
using PresentationLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace PresentationLayer
{
    public class PresentationService : IPresentationService
    {
        IBusinessService _buisnessService;
        private InformationForUser _userInformation;

        public PresentationService(IBusinessService businessService)
        {
            _buisnessService = businessService;
            _userInformation = new InformationForUser();
        }

        int activeUserRequest;

        public void ShowStartMenu()
        {
            const int menuExitItem = 7;

            do
            {
                var options = new List<string> { "Upload file to storage", "Download file from storage", "Rename file into storage", "Remove file from storage", "Show file info",
                                                 "Show user info", "Find file", "Exit " };

                activeUserRequest = ShowActionMenu(options);
                _UseRequestProcessing(activeUserRequest);
            }

            while (activeUserRequest != menuExitItem);
        }

        private void _UseRequestProcessing(int userRequest)
        {
            switch (userRequest)
            {
                case 0:
                    _GetInformationFromUserToUploadFile();
                    break;
                case 1:
                    _GetInformationFromUserToDownload();
                    break;
                case 2:
                    _GetInformationFromUserToRenameFile();
                    break;
                case 3:
                    _GetInformationFromUserToRemoveFile();
                    break;
                case 4:
                    _ShowInformationAboutFile();
                    break;
                case 5:
                    _ShowUserInfo();
                    break;
                case 6:
                    _ShowSearchResult();
                    break;
            }
        }

        private void _GetInformationFromUserToUploadFile()
        {

            Console.WriteLine(@"Enter the path to the file or enter ""back"" to return to the menu");
            string pathToFile = Console.ReadLine();

            if (pathToFile != "back")
            {
                _ShowFileUploadResult(pathToFile);
            }
            else
            {
                Console.Clear();
            }
        }

        private void _ShowFileUploadResult(string pathToFile)
        {
            _userInformation = _buisnessService.FileUploadCheck(pathToFile);

            _ShowMessage(_userInformation.Message);
        }

        private void _GetInformationFromUserToDownload()
        {
            string unloadingFile = _SelectionFileInStorage("Select the file you want to download");

            Console.WriteLine(@"enter the path to the download folder or enter ""back"" to return to the menu");
            string folderForUnloading = Console.ReadLine();

            if (folderForUnloading != "back")
            {
                _ShowFileDownloadResult(unloadingFile, folderForUnloading);
            }
            else
            {
                Console.Clear();
            }
        }

        private void _ShowFileDownloadResult(string unloadingFile, string folderForUnloading)
        {
            _userInformation = _buisnessService.FileDownloadCheck(unloadingFile, folderForUnloading);

            _ShowMessage(_userInformation.Message);
        }

        private void _ShowSearchResult()
        {
            Console.WriteLine("Enter file name");
            string fileName = Console.ReadLine();
            string pathToFile = (ConfigurationManager.AppSettings.Get("storageAddress")) + "\\" + fileName;
            string messegeForUser = _buisnessService.FileSearch(pathToFile) ? ($"File {fileName} is contained in the storage. Press any key to return to the menu") :
                                                                              ($"File {fileName} not found. Press any key to return to the menu");
            Console.Clear();
            Console.WriteLine(messegeForUser); Console.ReadKey();
            Console.Clear();
        }

        private void _ShowUserInfo()
        {
            Console.WriteLine($"Login: {ConfigurationManager.AppSettings.Get("login")}");
            Console.WriteLine($"Creation Date: {ConfigurationManager.AppSettings.Get("creationDate")}");
            Console.WriteLine($"Storage used: {_buisnessService.GetFileStorageSize()} byte");

            Console.ReadKey();
            Console.Clear();
        }

        private void _ShowInformationAboutFile()
        {
            string pathToFile = _SelectionFileInStorage("Select file to show information about");
            FileMetaInformation informationAboutselectedFile = _buisnessService.GetMetainformationAboutFile(pathToFile);

            Console.WriteLine($"File name: {informationAboutselectedFile.name}");
            Console.WriteLine($"file extension: {informationAboutselectedFile.extension}");
            Console.WriteLine($"File size: {informationAboutselectedFile.size} byte");
            Console.WriteLine($"Date of upload: {informationAboutselectedFile.creationDate}");
            Console.WriteLine($"Count of downloads: {informationAboutselectedFile.downloadСounter}");
            Console.WriteLine("Press any key to return to the menu");

            Console.ReadKey();
            Console.Clear();
        }

        private void _GetInformationFromUserToRemoveFile()
        {
            string pathToFile = _SelectionFileInStorage("Select file to delete");

            _ShowFileDeletionResult(pathToFile);
        }

        private void _ShowFileDeletionResult(string pathToFile)
        {

            _userInformation = _buisnessService.RemoveFileFromStorage(pathToFile);

            _ShowMessage(_userInformation.Message);
        }

        private void _GetInformationFromUserToRenameFile()
        {
            string oldName = _SelectionFileInStorage("Select file you want to rename");

            Console.WriteLine("Inter a new file name");
            string newName = Console.ReadLine();

            _ShowFileRenameResult(oldName, newName);
        }

        private void _ShowFileRenameResult(string oldName, string newName)
        {
            _userInformation = _buisnessService.FileRenameCheck(oldName, newName);

            _ShowMessage(_userInformation.Message);
        }

        private string _SelectionFileInStorage(string userMessage)
        {
            List<string> filesInStorage = new List<string>(Directory.GetFiles(ConfigurationManager.AppSettings.Get("storageAddress")));

            if (filesInStorage.Count == 0)
            {
                Console.WriteLine("You haven't uploaded any files to the repository yet. Press any key to return to the menu");
                Console.ReadKey();
                Console.Clear();

                ShowStartMenu();
            }
            Console.WriteLine(userMessage);

            return filesInStorage[ShowActionMenu(filesInStorage)];
        }

        private static int ShowActionMenu(List<string> listOfOptions, int activeMenuOption = 0)
        {
            bool isСhoiceMade;

            do
            {
                for (int i = 0; i < listOfOptions.Count; i++)
                {

                    if (activeMenuOption == i)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(listOfOptions[i]);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine(listOfOptions[i]);
                    }
                }

                activeMenuOption = GetUserChoiceForActionMenu(out isСhoiceMade, activeMenuOption);

                if (activeMenuOption >= listOfOptions.Count)
                {
                    activeMenuOption = 0;
                }
                else if (activeMenuOption < 0)
                {
                    activeMenuOption = listOfOptions.Count - 1;
                }
            }
            while (!isСhoiceMade);

            return activeMenuOption;
        }

        private static int GetUserChoiceForActionMenu(out bool IsСhoiceMade, int activeMenuOption)
        {
            var userAction = Console.ReadKey().Key;

            switch (userAction)
            {
                case ConsoleKey.DownArrow:
                    activeMenuOption += 1;
                    Console.Clear();
                    IsСhoiceMade = false;
                    return activeMenuOption;

                case ConsoleKey.UpArrow:
                    activeMenuOption -= 1;
                    Console.Clear();
                    IsСhoiceMade = false;
                    return activeMenuOption;

                case ConsoleKey.Enter:
                    Console.Clear();
                    IsСhoiceMade = true;
                    return activeMenuOption;

                default:
                    Console.Clear();
                    IsСhoiceMade = false;
                    return activeMenuOption;
            }
        }

        public void PreparationForFirstLaunch()
        {
            GetInformationFromUserToCreateFileStorage();

            if (!_buisnessService.IsBinaryRepositoryExists())
            {
                _buisnessService.CreateBinaryRepository();
            }
            Environment.Exit(0);
        }

        private void GetInformationFromUserToCreateFileStorage()
        {
            Console.WriteLine("Come up with a name for the folder that will be used as file storage");
            string storageName = Console.ReadLine();

            Console.WriteLine($"Specify the location where the folder {storageName} will be created");
            string pathToStorage = Console.ReadLine();

            _СheckPossibilityCreatingFileStorage(pathToStorage + "\\" + storageName, pathToStorage);
        }

        private void _СheckPossibilityCreatingFileStorage(string storageName, string pathToStorage)
        {
            _userInformation = _buisnessService.CreateFileStorage(storageName, pathToStorage);

            if (_userInformation.IsOperationValid)
            {
                Console.WriteLine(_userInformation.Message);
            }

            else
            {
                Console.Clear();
                Console.WriteLine(_userInformation.Message);
                GetInformationFromUserToCreateFileStorage();
            }
        }

        private void _ShowMessage(string message)
        {
            Console.Clear();
            Console.WriteLine(message);
            Console.ReadKey();
            Console.Clear();
        }
    }
}

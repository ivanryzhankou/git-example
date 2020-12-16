using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using lab_02.DataLayer.Models;


namespace lab_02.PresentationLayer
{
    class PresentationService
    {
        private readonly BuisnessLayer.BuisnessService _buisnessService;
        private InformationForUser _userInformation;

        public PresentationService()
        {
            _buisnessService = new BuisnessLayer.BuisnessService();
            _userInformation = new InformationForUser();
        }

        int activeUserRequest;

        public void ShowStartMenu()
        {
            const int menuExitItem = 7;

            do
            {
                var options = new List<string> { "Upload file to storage", "Unload file from storage", "Rename file into storage", "Remove file from storage", "Show file info",
                                                 "Show user info", "Find file", "Exit" };

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
            FileMetaInformation informationAboutselectedFile = _buisnessService.GetInformationAboutFile(pathToFile);

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

            Console.Clear();
            Console.WriteLine(_userInformation.informationForUser);
            Console.ReadKey();
            Console.Clear();
        }

        private void _GetInformationFromUserToRenameFile()
        {
            string oldName = _SelectionFileInStorage("Select file you want to rename");

            Console.WriteLine("Inter a new file name");

            string newName = Console.ReadLine();

            _ShowFileRenameCheck(oldName, newName);

        }

        private void _ShowFileRenameCheck(string oldName, string newName)
        {
            _userInformation = _buisnessService.CheckingRenameFile(oldName, newName);

            if (_userInformation.isOperationValid)
            {
                _ShowFileRenameResult(oldName, newName);
            }
            else
            {
                Console.Clear();
                Console.WriteLine(_userInformation.informationForUser);
                _GetInformationFromUserToRenameFile();
            }
        }

        private void _ShowFileRenameResult(string oldName, string newName)
        {
            _userInformation = _buisnessService.RenameFile(oldName, newName);

            Console.Clear();
            Console.WriteLine(_userInformation.informationForUser);
            Console.ReadKey();
            Console.Clear();
        }
        private void _GetInformationFromUserToUploadFile()
        {
            string pathToFile = _GetPathToFile(@"Enter the path to the file or enter ""back"" to return to the menu");

            if (pathToFile != "back")
            {
                _ShowFileUploadCheck(pathToFile);
            }
            else
            {
                Console.Clear();
            }
        }

        private void _ShowFileUploadCheck(string pathToFile)
        {
            _userInformation = _buisnessService.CheckFileUpload(pathToFile);

            if (_userInformation.isOperationValid)
            {
                _ShowResultExecutionFileUpload(pathToFile);
            }

            else
            {
                Console.Clear();
                Console.WriteLine(_userInformation.informationForUser);

                _GetInformationFromUserToUploadFile();
            }
        }

        private void _ShowResultExecutionFileUpload(string pathToFile)
        {
            Console.Clear();

            _userInformation = _buisnessService.UploadFileIntoStorage(pathToFile);

            Console.WriteLine(_userInformation.informationForUser);
            Console.ReadKey();
            Console.Clear();
        }

        private void _GetInformationFromUserToDownload()
        {
            string unloadingFile = _SelectionFileInStorage("Select the file you want to download");
            string folderForUnloading = _GetPathToFile(@"enter the path to the unload folder or enter ""back"" to return to the menu");

            if (folderForUnloading != "back")
            {
                _ShowFileDownloadCheck(unloadingFile, folderForUnloading);
            }
            else
            {
                Console.Clear();
            }
        }

        private void _ShowFileDownloadCheck(string unloadingFile, string folderForUnloading)
        {
            _userInformation = _buisnessService.CheckFileForUnload(unloadingFile, folderForUnloading);

            if (_userInformation.isOperationValid & !_userInformation.needReplacement)
            {
                _ShowDownloadFileResul(unloadingFile, folderForUnloading);
            }

            else if (_userInformation.needReplacement)
            {
                Console.Clear();
                Console.WriteLine(_userInformation.informationForUser);

                List<string> options = new List<string>() { "Yes", "No" };
                int userChois = ShowActionMenu(options);

                switch (userChois)
                {
                    case 0:
                        _ShowDownloadFileResul(unloadingFile, folderForUnloading);
                        break;
                    case 1:
                        ShowStartMenu();
                        break;
                }
            }

            else
            {
                Console.Clear();
                Console.WriteLine(_userInformation.informationForUser);

                _GetInformationFromUserToDownload();
            }
        }

        private void _ShowDownloadFileResul(string unloadingFile, string folderForUnloading)
        {
            Console.Clear();

            _userInformation = _buisnessService.DownloadFilesIntoStorage(unloadingFile, folderForUnloading);

            Console.WriteLine(_userInformation.informationForUser);
            Console.ReadKey();
            Console.Clear();
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

        private string _GetPathToFile(string userMessage)
        {
            Console.WriteLine(userMessage);
            string pathToFile = @"" + Console.ReadLine();

            return pathToFile;
        }

        internal static int ShowActionMenu(List<string> listOfOptions, int activeMenuOption = 0)
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

        internal static int GetUserChoiceForActionMenu(out bool IsСhoiceMade, int activeMenuOption)
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

        internal void PreparationForFirstLaunch()
        {
            GetInformationFromUserToCreateFileStorage();

            if (!_buisnessService.IsBinaryRepositoryExists())
            {
                _buisnessService.CreateBinaryRepository();
            }
            Environment.Exit(0);
        }

        internal void GetInformationFromUserToCreateFileStorage()
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

            if (_userInformation.isOperationValid)
            {
                Console.WriteLine(_userInformation.informationForUser);
            }

            else
            {
                Console.Clear();
                Console.WriteLine(_userInformation.informationForUser);
                GetInformationFromUserToCreateFileStorage();
            }
        }
    }
}


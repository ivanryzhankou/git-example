using System;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using lab_02.DataLayer.Models;
using lab_02.BuisnessLayer.Model;


namespace lab_02.InterfaceLayer
{
    class InterfaceService
    {
        //todo: private fields should start with _, like _userInformation
        //todo: it is better to instantiate fields in class ctor
        BuisnessLayer.BuisnessService buisnessService = new BuisnessLayer.BuisnessService();
        InformationForUser userInformation = new InformationForUser();


        int activeUserRequest;

        public void ShowStartMenu()
        {
            do
            {
                //todo: var would be better here as it is clear what type is used from the new List<string> part. No need to repeat yourself
                //todo: no need to call a ctor if you instantiate variable(remove () here)
                //todo: please align the list's values so it is convenient to read
                List<string> options = new List<string>() { "Upload file to storage", "Unload file from storage", "Rename file into storage", "Remove file from storage", "Show file info",
                    "Show user info", "Find file", "Exit" };

                activeUserRequest = ShowActionMenu(options);
                UseRequestProcessing(activeUserRequest);
            }

            while (activeUserRequest != 7);//todo: why 7? it looks like magic number and hard to understand from code what it means
        }

        private void UseRequestProcessing(int userRequest)
        {
            switch (userRequest)
            {
                case 0:
                    GetInformationFromUserToUploadFile();
                    break;
                case 1:
                    GetInformationFromUserToUnloading();
                    break;
                case 2:
                    GetInformationFromUserToRenameFile();
                    break;
                case 3:
                    GetInformationFromUserToRemoveFile();
                    break;
                case 4:
                    GetInformationFromUserAboutFile();
                    break;
                case 5:
                    ShowUserInfo();
                    break;
                case 6:
                    ShowSearchResult();
                    break;
                //todo: the default case is useless and can be removed
                default:
                    break;
            }
        }

        private void ShowSearchResult ()
        {
            UserNotice("Enter file name");
            string fileName =  GetStringFromUser();

            string pathToFile = (ConfigurationManager.AppSettings.Get("storageAddress")) + "\\" + fileName;
           
            //todo: these two if/else statements differ only in message. Can be easily substituted with ternary operation
            if (buisnessService.FileSearch(pathToFile))
            {
                Console.Clear();
                UserNotice($"File {fileName} is contained in the storage. Press any key to return to the menu");
                Console.ReadKey();
                Console.Clear();
            }

            else
            {
                Console.Clear();
                UserNotice($"File {fileName} not found. Press any key to return to the menu");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private void ShowUserInfo()
        {
            UserNotice($"Login: {ConfigurationManager.AppSettings.Get("login")}");
            UserNotice($"Creation Date: {ConfigurationManager.AppSettings.Get("creationDate")}");
            UserNotice($"Storage used: {buisnessService.GetFileStorageSize()} byte");

            Console.ReadKey();
            Console.Clear();
        }

        private void GetInformationFromUserAboutFile()
        {
            string pathToFile = SelectionFileInStorage("Select file to show information about");
            FileMetaInformation informationAboutselectedFile = buisnessService.GetInformationAboutFile(pathToFile);

            ShowInformationAboutFile(informationAboutselectedFile);
        }

        private void ShowInformationAboutFile(FileMetaInformation selectedFile) 
        {
            //todo: prefer to use string interpolation when it is possible
            UserNotice("File name: " + selectedFile.name);
            UserNotice("file extension: " + selectedFile.extension);
            UserNotice("File size: " + selectedFile.size + " byte");
            UserNotice("Date of upload: " + selectedFile.creationDate);
            UserNotice("Count of downloads: " + selectedFile.downloadСounter);
            UserNotice("Press any key to return to the menu");

            Console.ReadKey();
            Console.Clear();
        }

        private void GetInformationFromUserToRemoveFile()
        {
            string pathToFile = SelectionFileInStorage("Select file to delete");

            ShowResultOfFileDeletion(pathToFile);
        }

        private void ShowResultOfFileDeletion(string pathToFile)
        {

            userInformation = buisnessService.RemoveFileFromStorage(pathToFile);

            Console.Clear();

            UserNotice(userInformation.informationForUser);

            Console.ReadKey();
            Console.Clear();
        }

        private void GetInformationFromUserToRenameFile()
        {
            string oldName = SelectionFileInStorage("Select file you want to rename");
            UserNotice("Inter a new file name");
            string newName = GetStringFromUser();

            ShowErrorWhenRenamingFile(oldName, newName);

        }

        private void ShowErrorWhenRenamingFile(string oldName, string newName)
        {
            userInformation = buisnessService.CheckingRenameFile(oldName, newName);

            if (userInformation.isOperationValid)
            {
                ShowResultOfFileRename(oldName, newName);
            }
            else
            {
                Console.Clear();
                UserNotice(userInformation.informationForUser);
                GetInformationFromUserToRenameFile();
            }
        }

        private void ShowResultOfFileRename(string oldName, string newName)
        {
            userInformation = buisnessService.RenameFile(oldName, newName);
            Console.Clear();
            UserNotice(userInformation.informationForUser);
            Console.ReadKey();
            Console.Clear();
        }
        private void GetInformationFromUserToUploadFile()
        {
            string pathToFile = GetPathToFile(@"Enter the path to the file or enter ""back"" to return to the menu");

            if (pathToFile != "back")
            {
                ShowResultOfFileUploadingChecking(pathToFile);
            }
            else
            {
                Console.Clear();
            }
        }

        private void ShowResultOfFileUploadingChecking(string pathToFile)
        {
            userInformation = buisnessService.CheckingFileUpload(pathToFile);

            if (userInformation.isOperationValid)
            {
                ShowResultExecutionFileUpload(pathToFile);
            }

            else
            {
                Console.Clear();
                UserNotice(userInformation.informationForUser);
                GetInformationFromUserToUploadFile();
            }
        }

        private void ShowResultExecutionFileUpload(string pathToFile)
        {
            Console.Clear();

            userInformation = buisnessService.UploadFileIntoStorage(pathToFile);

            UserNotice(userInformation.informationForUser);

            Console.ReadKey();
            Console.Clear();
        }

        private void GetInformationFromUserToUnloading()
        {
            string unloadingFile = SelectionFileInStorage("Select the file you want to download");
            string folderForUnloading = GetPathToFile(@"enter the path to the unload folder or enter ""back"" to return to the menu");

            if (folderForUnloading != "back")
            {
                ShowResultOfFileUnloadingChecking(unloadingFile, folderForUnloading);
            }
            else
            {
                Console.Clear();
            }
        }

        private void ShowResultOfFileUnloadingChecking(string unloadingFile, string folderForUnloading)
        {
            userInformation = buisnessService.CheckFileForUnload(unloadingFile, folderForUnloading);

            if (userInformation.isOperationValid & !userInformation.needReplacement)
            {
                ShowResultExecutionFileUnloading(unloadingFile, folderForUnloading);
            }

            else if (userInformation.needReplacement)
            {
                Console.Clear();
                UserNotice(userInformation.informationForUser);
                List<string> options = new List<string>() { "Yes", "No" };
                int userChois = ShowActionMenu(options);

                switch (userChois)
                {
                    case 0:
                        ShowResultExecutionFileUnloading(unloadingFile, folderForUnloading);
                        break;
                    case 1:
                        ShowStartMenu();
                        break;
                }
            }

            else
            {
                Console.Clear();
                UserNotice(userInformation.informationForUser);
                GetInformationFromUserToUnloading();
            }
        }

        private void ShowResultExecutionFileUnloading(string unloadingFile, string folderForUnloading)
        {
            Console.Clear();

            userInformation = buisnessService.UnloadFilesIntoStorage(unloadingFile, folderForUnloading);

            UserNotice(userInformation.informationForUser);

            Console.ReadKey();
            Console.Clear();
        }

        private string SelectionFileInStorage(string userMessage)
        {
            List<string> filesInStorage = new List<string>(Directory.GetFiles(ConfigurationManager.AppSettings.Get("storageAddress")));

            if (filesInStorage.Count == 0)
            {
                UserNotice("You haven't uploaded any files to the repository yet. Press any key to return to the menu");
                Console.ReadKey();
                Console.Clear();

                ShowStartMenu();
            }
            UserNotice(userMessage);

            return filesInStorage[ShowActionMenu(filesInStorage)];
        }

        private string GetPathToFile(string userMessage)
        {
            UserNotice(userMessage);
            string pathToFile = @"" + GetStringFromUser();

            return pathToFile;
        }

        internal static int ShowActionMenu(List<string> listOfOptions, int activeMenuOption = 0)
        {
            //todo: variables must start from lower case
            //todo: false is default value for bool, no need to asign it
            bool IsСhoiceMade = false;

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

                activeMenuOption = GetUserChoiceForActionMenu(out IsСhoiceMade, activeMenuOption);

                if (activeMenuOption >= listOfOptions.Count)
                {
                    activeMenuOption = 0;
                }
                else if (activeMenuOption < 0)
                {
                    activeMenuOption = listOfOptions.Count - 1;
                }
            }
            while (!IsСhoiceMade);

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


        //todo: it's up to you but I think the UserNotice and GetStringFromUser are useless
        internal static void UserNotice(string message)
        {
            {
                Console.WriteLine(message);
            }
        }
        
        //todo: string is a type, so naming is not accurate here. Something like GetUserInput would be better
        internal static string GetStringFromUser()
        {
            {
                return Console.ReadLine();
            }
        }

        internal void PreparationForFirstLaunch()
        {
            GetInformationFromUserToCreateFileStorage();

            if (!buisnessService.CheckForExistenceOfBinaryRepository())
            {
                buisnessService.CreateBinaryRepository();
            }
            Environment.Exit(0);
        }

        internal void GetInformationFromUserToCreateFileStorage()
        {
            UserNotice("Come up with a name for the folder that will be used as file storage");
            string storageName = GetStringFromUser();

            UserNotice($"Specify the location where the folder {storageName} will be created");
            string pathToStorage = GetStringFromUser();

            СheckPossibilityCreatingFileStorage(pathToStorage + "\\" + storageName, pathToStorage);
        }

        private void СheckPossibilityCreatingFileStorage(string storageName, string pathToStorage)
        {
            userInformation = buisnessService.CreateFileStorage(storageName, pathToStorage);

            if (userInformation.isOperationValid)
            {
                UserNotice(userInformation.informationForUser);
            }

            else
            {
                Console.Clear();
                UserNotice(userInformation.informationForUser);
                GetInformationFromUserToCreateFileStorage();
            }
        }
    }
}


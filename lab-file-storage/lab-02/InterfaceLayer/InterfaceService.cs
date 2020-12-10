using System;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using lab_02.DataLayer.Models;




namespace lab_02.InterfaceLayer
{
    class InterfaceService
    {
        BuisnessLayer.BuisnessService buisnessService = new BuisnessLayer.BuisnessService();
        BuisnessLayer.Model.InformationForUser userInformation = new BuisnessLayer.Model.InformationForUser();


        int activeUserRequest;

        public void ShowStartMenu()
        {
            do
            {
                List<string> options = new List<string>() { "Upload file to storage", "Unload file from storage", "Rename file into storage", "Remove file from storage", "Show file info",
                    "Show user info", "Exit" };

                activeUserRequest = ShowActionMenu(options);
                UseRequestProcessing(activeUserRequest);
            }

            while (activeUserRequest != 6);
        }

        private void UseRequestProcessing(int userRequest)
        {
            switch (userRequest)
            {
                case 0:
                    GetInformationToUploadFileFromUser();
                    break;
                case 1:
                    GetInformationToUnloadingFromUser();
                    break;
                case 2:
                    GetInformationToRenameFileFromUser();
                    break;
                case 3:
                    GetInformationToDeleteFilFromUsere();
                    break;
                case 4:
                    GetInformationAboutFileFromUser();
                    break;
                default:
                    break;
            }
        }

        private void GetInformationAboutFileFromUser()
        {
            string pathToFile = SelectionFileInStorage("Select file to show information about");
            FileMetaInformation informationAboutselectedFile = buisnessService.GetInformationAboutFile(pathToFile);

            ShowInformationAboutFile(informationAboutselectedFile);
        }

        private void ShowInformationAboutFile(FileMetaInformation selectedFile) 
        {
            UserNotice("File name: " + selectedFile.name);
            UserNotice("file extension: " + selectedFile.extension);
            UserNotice("File size: " + selectedFile.size + " byte");
            UserNotice("Date of upload: " + selectedFile.creationDate);
            UserNotice("Count of downloads: " + selectedFile.downloadsNumber);
            UserNotice("Press any key to return to the menu");
            Console.ReadKey();
            Console.Clear();
        }

        private void GetInformationToDeleteFilFromUsere()
        {
            string pathToFile = SelectionFileInStorage("Select file to delete");

            ShowResultOfFileDeletion(pathToFile);
        }

        private void ShowResultOfFileDeletion(string pathToFile)
        {

            userInformation = buisnessService.DeleteFileFromStorage(pathToFile);

            Console.Clear();
            UserNotice(userInformation.informationForUser);
            Console.ReadKey();
            Console.Clear();
        }

        private void GetInformationToRenameFileFromUser()
        {
            string oldName = SelectionFileInStorage("Select file you want to rename");
            UserNotice("Inter a new file name");
            string newName = GetStringFromUser();

            ShowErrorWhenRenamingFile(oldName, newName);

        }

        private void ShowErrorWhenRenamingFile(string oldName, string newName)
        {
            userInformation = buisnessService.CheckingRenameFile(oldName, newName);

            if (userInformation.isFileValid)
            {
                ShowResultOfFileRename(oldName, newName);
            }
            else
            {
                Console.Clear();
                UserNotice(userInformation.informationForUser);
                GetInformationToRenameFileFromUser();
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
        private void GetInformationToUploadFileFromUser()
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

            if (userInformation.isFileValid)
            {
                ShowResultExecutionFileUpload(pathToFile);
            }

            else
            {
                Console.Clear();
                UserNotice(userInformation.informationForUser);
                GetInformationToUploadFileFromUser();
            }
        }

        private void ShowResultExecutionFileUpload(string pathToFile)
        {
            Console.Clear();
            UserNotice(buisnessService.UploadFileIntoStorage(pathToFile));
            Console.ReadKey();
            Console.Clear();
        }

        private void GetInformationToUnloadingFromUser()
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

            if (userInformation.isFileValid & !userInformation.needReplacement)
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
                GetInformationToUnloadingFromUser();
            }
        }

        private void ShowResultExecutionFileUnloading(string unloadingFile, string folderForUnloading)
        {
            Console.Clear();
            UserNotice(buisnessService.UnloadFilesIntoStorage(unloadingFile, folderForUnloading));
            Console.ReadKey();
            Console.Clear();
        }

        private string SelectionFileInStorage(string userMessage)
        {
            List<string> filesInStorage = new List<string>(Directory.GetFiles(ConfigurationManager.AppSettings.Get("storageAddress")));
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

        internal static void UserNotice(string message)
        {
            {
                Console.WriteLine(message);
            }
        }

        internal static string GetStringFromUser()
        {
            {
                return Console.ReadLine();
            }
        }

    }
}


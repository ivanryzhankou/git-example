using System;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;
using System.Collections.Generic;
using System.Linq;



namespace lab_02.InterfaceLayer
{
    class InterfaceService
    {
        BuisnessLayer.BuisnessService buisnessService = new BuisnessLayer.BuisnessService();
        BuisnessLayer.Model.UserInformation userInformation = new BuisnessLayer.Model.UserInformation();


        int activeUserRequest;

        public void ShowStartMenu()
        {
            do
            {
                List<string> options = new List<string>() { "Download files to storage", "Save files from storage", "Rename file into storage", "Show user info", "Exit" };

                activeUserRequest = ShowActionMenu(options);
                UseRequestProcessing(activeUserRequest);
            }

            while (activeUserRequest != 4);
        }

        private void UseRequestProcessing(int userRequest)
        {
            switch (userRequest)
            {
                case 0:
                    getInformationToUploadFile();
                    break;
                case 1:
                    getInformationToUnloading();
                    break;
                case 2:
                    getInformationToRenameFile();
                    break;
                default:
                    break;
            }

        }

        private void getInformationToRenameFile()
        {
            string oldName = SelectionFileInStorage("Select the file you want to rename");
            UserNotice("Inter a new file name");
            string newName = GetStringFromUser();

            ShowRenameResult(oldName, newName);

        }

        private void ShowRenameResult(string oldName, string newName)
        {

        }
        private void getInformationToUploadFile()
        {
            string pathToFile = GetPathToFile(@"Enter the path to the file or enter ""back"" to return to the menu");

            if (pathToFile != "back")
            {
                ShowResultOfFileUpload(pathToFile);
            }
            else
            {
                Console.Clear();
            }
        }

        private void getInformationToUnloading()
        {
            string unloadingFile = SelectionFileInStorage("Select the file you want to download");
            string folderForUnloading = GetPathToFile(@"enter the path to the unload folder or enter ""back"" to return to the menu");

            if (folderForUnloading != "back")
            {
                ShowResultOfFileUnloading(unloadingFile, folderForUnloading);
            }
            else
            {
                Console.Clear();
            }
        }

        private void ShowResultOfFileUnloading(string unloadingFile, string folderForUnloading)
        {
            userInformation = buisnessService.UnloadFilesIntoStorage(unloadingFile, folderForUnloading);

            if (userInformation.isFileValid)
            {
                Console.Clear();
                UserNotice(userInformation.informationForUser);
                Console.ReadKey();
                Console.Clear();
                ShowStartMenu();
            }

            if (userInformation.needReplacement)
            {
                Console.Clear();
                UserNotice(userInformation.informationForUser);
                List<string> options = new List<string>() {"Yes","No" };
                ShowActionMenu(options);
            }

            else
            {
                Console.Clear();
                UserNotice(userInformation.informationForUser);
                getInformationToUnloading();
            }
        }

        private string SelectionFileInStorage(string userMessage)
        {
            List<string> filesInStorage = new List<string>(Directory.GetFiles(ConfigurationManager.AppSettings.Get("storageAddress")));
            UserNotice(userMessage);

            return filesInStorage[ShowActionMenu(filesInStorage)];
        }

        private void ShowResultOfFileUpload(string pathToFile)
        {
            userInformation = buisnessService.CheckingFileUpload(pathToFile); 


            if (!userInformation.isFileValid)
            {
                Console.Clear();
                UserNotice(userInformation.informationForUser);
                getInformationToUploadFile();
            }

            else
            {
                Console.Clear();
                UserNotice(userInformation.informationForUser);
                Console.ReadKey();

            }
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


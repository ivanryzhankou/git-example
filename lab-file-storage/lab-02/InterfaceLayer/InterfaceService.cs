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

        int activeUserRequest;

        public void ShowStartMenu()
        {
            do
            {
                List<string> options = new List<string>() { "Download files to storage", "Save files from storage", "Show user info", "Exit" };

                activeUserRequest = ShowActionMenu(options);
                UseRequestProcessing(activeUserRequest);
            }

            while (activeUserRequest != 3);
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
                    break;
                default:
                    break;
            }
        }
        internal void ShowFileMetaInformation(string fileName)
        {
            Dictionary<string, Models.FileMetaInformation> metaInformationFiles = DeserializeFileMetaInformation();

            Models.FileMetaInformation activeFile = metaInformationFiles.GetValueOrDefault(fileName);

            Console.WriteLine("File name: " + activeFile.name);
            Console.WriteLine("file extension: " + activeFile.extension);
            Console.WriteLine("File size: " + activeFile.size + " kb");
            Console.WriteLine("Date of upload: " + activeFile.creationDate);
            Console.WriteLine("Number of downloads: " + activeFile.downloadsNumber);
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

        private void ShowResultOfFileUnloading (string unloadingFile, string folderForUnloading)
        {
            var resultOfChecking = (isFileValid: true, fileNeedReplacment: false, downloadResultMessage: string.Empty);

            resultOfChecking = buisnessService.FileUnloadCheck(unloadingFile, folderForUnloading);

            if (!resultOfChecking.isFileValid && !resultOfChecking.fileNeedReplacment)
            {
                Console.Clear();
                UserNotice(resultOfChecking.downloadResultMessage);
                getInformationToUploadFile();
            }

            if (resultOfChecking.fileNeedReplacment)
            {

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
            var resultOfChecking = (isFileValid: true, downloadResultMessage: string.Empty);

            resultOfChecking = buisnessService.FileUploadCheck(pathToFile);

            if (!resultOfChecking.isFileValid)
            {
                Console.Clear();
                UserNotice(resultOfChecking.downloadResultMessage);
                getInformationToUploadFile();
            }

            else
            {
                Console.Clear();
                UserNotice(resultOfChecking.downloadResultMessage);
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


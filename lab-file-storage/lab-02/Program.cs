using System;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;
using System.Collections.Generic;
using System.Linq;


namespace lab_02
{
    class Program
    {
        static void Main(string[] args)
        {
            LoadingIntoStorage();
        }

        private static void LoadingIntoStorage()
        {
            int maxFileSize = 150; // Megabyte
            int MaximumStorageSize = 10240; // не забыть убрать отсюда!!!!
            bool isFileValid = true;

            string pathToFile = string.Empty;
            do
            {
                UserNotice("Enter the path to the file");
                pathToFile = @"" + GetStringFromUser();

                if (File.Exists(pathToFile) == false)
                {
                    Console.Clear();
                    UserNotice("This file does not exist. Please try again");
                    isFileValid = false;
                }

                else if (GetFfileSize(pathToFile) >= maxFileSize)
                {
                    Console.Clear();
                    UserNotice("Sorry.The file cannot be larger than 150 MB");
                    isFileValid = false;
                }

                else if (GetFolderSize(ConfigurationManager.AppSettings.Get("storageAddress")) + GetFfileSize(pathToFile) > MaximumStorageSize)
                {
                    Console.Clear();
                    UserNotice("Sorry.You cannot store more than 10 gigabytes.Pay for an increase in available storage or select a different file");
                    isFileValid = false;
                }

                else if (checkUniquenessFilename(pathToFile) == false)
                {
                    Console.Clear();
                    UserNotice("Sorry.You cannot store more than 10 gigabytes.Pay for an increase in available storage or select a different file");
                    isFileValid = false;
                }
            }

            while (!isFileValid);
            {
                if (isFileValid == true)
                {
                    FileInfo fileInf = new FileInfo(pathToFile);

                    string a = fileInf.Name;

                    Console.WriteLine(a);
                   

                    fileInf.CopyTo((ConfigurationManager.AppSettings.Get("storageAddress") + "\\" + fileInf.Name));
                }
            }
        }

        private static bool checkUniquenessFilename(string pathToFile)
        {
            List<string> files = new List<string>(Directory.GetFiles(ConfigurationManager.AppSettings.Get("storageAddress")));
            FileInfo File = new FileInfo(pathToFile);

            for (int i = 0; i < files.Count; i++)
            {
                if (File.Name == (files[i].Remove(0, (ConfigurationManager.AppSettings.Get("storageAddress").Length + 1))))
                {
                    return false;
                }
            }

            return true;
        }

        private static double GetFfileSize(string pathToFile)
        {
            FileInfo File = new FileInfo(pathToFile);

            Console.WriteLine(conversionToMegabytes(File.Length));
            return conversionToMegabytes(File.Length);
        }

        private static double GetFolderSize(string pathToFolder)
        {
            long folderSize = 0;

            List<string> files = new List<string>(Directory.GetFiles(pathToFolder));

            foreach (string file in files)
            {
                folderSize += file.Length;
            }
            return conversionToMegabytes(folderSize);
        }


        public static double conversionToMegabytes(long bytes)
        {
            const float unitMeasurement = 1048576; // Megabyte

            return Math.Round(bytes / unitMeasurement, 6);
        }

        private static List<string> GetFolderContents(string path)
        {
            List<string> folders = new List<string>(Directory.GetFiles(path));
            List<string> files = new List<string>(Directory.GetDirectories(path));
            List<string> foldersAndFiles = new List<string>();

            foldersAndFiles.AddRange(folders);
            foldersAndFiles.AddRange(files);

            for (int i = 0; i < foldersAndFiles.Count; i++)
            {
                foldersAndFiles[i] = foldersAndFiles[i].Substring(path.Length);
            }

            return foldersAndFiles;
        }

        private static int ShowActiveMenu(List<string> listOfOptions, int activeOption = 0)
        {
            var userAction = ConsoleKey.Spacebar;
            do
            {
                for (int i = 0; i < listOfOptions.Count; i++)
                {
                    if (activeOption == i)
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

                userAction = Console.ReadKey().Key;

                switch (userAction)
                {
                    case ConsoleKey.DownArrow:
                        activeOption += 1;
                        if (activeOption >= listOfOptions.Count) { activeOption = 0; };
                        Console.Clear();
                        break;

                    case ConsoleKey.UpArrow:
                        activeOption -= 1;
                        if (activeOption < 0) { activeOption = listOfOptions.Count - 1; };
                        Console.Clear();
                        break;
                    default:
                        Console.Clear();
                        break;
                }
            }
            while (userAction != ConsoleKey.Enter);

            return activeOption;
        }

        private static void ShowStarMenu()
        {
            List<string> options = new List<string>() { "ShowStorageContents", "Upload files to storage", "Show user info" };

            ShowActiveMenu(options);
        }

        private static bool UserVerification(string login, string password)
        {
            return login == ConfigurationManager.AppSettings.Get("login") && password == ConfigurationManager.AppSettings.Get("password");
        }

        private static void Logging()
        {
            bool isUserValid;

            do
            {
                UserNotice("Enter your login");
                string login = GetStringFromUser();

                UserNotice("Enter your password");
                string password = GetStringFromUser();

                isUserValid = UserVerification(login, password);

                if (isUserValid == false)
                {
                    Console.Clear();
                    UserNotice("Try again. Wrong login or password.");
                }
            }
            while (isUserValid == false);
            Console.Clear();
        }

        private static string GetStringFromUser()
        {
            {
                return Console.ReadLine();
            }
        }

        private static void UserNotice(string message)
        {
            {
                Console.WriteLine(message);
            }
        }

        //public static void SelectdownloadableFiles()
        //{
        //    string pathDownloadedFile = @"";
        //    string previousFolder;
        //    int userChois;
        //    int count = 0;

        //    List<string> contentsOfDirectory = new List<string>();
        //    contentsOfDirectory.AddRange(GetDrivesList());

        //    do
        //    {
        //        userChois = ShowActiveMenu(contentsOfDirectory);
        //        previousFolder = pathDownloadedFile;

        //        if (userChois == 0 && count !=0)
        //        {
        //            contentsOfDirectory.Clear();
        //            contentsOfDirectory.AddRange(GetFolderContents(previousFolder));
        //            contentsOfDirectory.Insert(0, "...");

        //        }
        //        else
        //        {
        //            pathDownloadedFile += contentsOfDirectory[userChois];
        //            contentsOfDirectory.Clear();
        //            contentsOfDirectory.AddRange(GetFolderContents(pathDownloadedFile));
        //            contentsOfDirectory.Insert(0, "...");
        //            count++;

        //        }
        //    }

        //    while (File.Exists(pathDownloadedFile) == false);
        //}

        //private static List<string> GetDrivesList()
        //{
        //    List<DriveInfo> allDrives = new List<DriveInfo>(DriveInfo.GetDrives());
        //    List<string> drivesList = new List<string>();

        //    for (int i = 0; i < allDrives.Count; i++)
        //    {
        //        drivesList.Add(allDrives[i].Name);
        //    }

        //    return drivesList;
        //}
    }
}

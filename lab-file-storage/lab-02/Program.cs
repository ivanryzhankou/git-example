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
            Logging();
            ShowStorageContents();
        }

        private static void ShowStorageContents ()
        {
            Directory.CreateDirectory(ConfigurationManager.AppSettings.Get("repositoryAddress"));
            List <string> filesInStorage = new List <string> (Directory.GetFiles(ConfigurationManager.AppSettings.Get("repositoryAddress")));
            foreach (string file in filesInStorage)
            {
                Console.WriteLine(file.Remove(0, (ConfigurationManager.AppSettings.Get("repositoryAddress").Length + 1)));
            }
        }

        private static int ShowMenu( List<string>listOfOptions)
        {
            int activeOption = 0;

            foreach (string file in listOfOptions)
            {
                Console.WriteLine(file);
            }
        }
        //private static void Test()
        //{
        //    Directory.CreateDirectory("c:\\Users\\i.ryzhankou\\Desktop\\test");
        //    Directory.Delete("c:\\Users\\i.ryzhankou\\Desktop\\test");
        //    string a = "c:\\Users\\i.ryzhankou\\Desktop\\test";
        //    var txtFiles = Directory.EnumerateFiles(a);

        //    foreach (string str in txtFiles)
        //    {
        //        Console.WriteLine(str.Remove(0, a.Length + 1));
        //    }
        //    Console.WriteLine("ok");
        //}

        //private static void UpdateSetting(string key, string value)
        //{
        //    Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //    configuration.AppSettings.Settings[key].Value = value;
        //    configuration.Save();

        //    ConfigurationManager.RefreshSection("appSettings");
        //}
        private static void Logging()
        {
            bool isUserValid; // Valid variable name?

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

            //if (ConfigurationManager.AppSettings.Get("repositoryAddress") == "null")
            //{
            //    RepositoryCreation();
            //}
        }

        //private static void RepositoryCreation()
        //{
        //    UpdateSetting("repositoryAddress", GetStringFromUser("enter repository address"));
        //}

        private static bool UserVerification(string login, string password)
        {
           return login == ConfigurationManager.AppSettings.Get("login") && password == ConfigurationManager.AppSettings.Get("password");
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
    }
}

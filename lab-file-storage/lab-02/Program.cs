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

            Console.WriteLine(ConfigurationManager.AppSettings.Get("repositoryAddress"));
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

        private static void UpdateSetting(string key, string value)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save();

            ConfigurationManager.RefreshSection("appSettings");
        }
        private static void Logging()
        {
            bool validUser; // Valid variable name?

            do
            {
                string login = GetStringFromUser("Enter your login");

                string password = GetStringFromUser("Enter your password");

                validUser = UserVerification(login, password);

                if (validUser == false)
                {
                    Console.Clear();
                    UserNotice("Try again. Wrong login or password.");
                }
            }

            while (validUser == false);

            if (ConfigurationManager.AppSettings.Get("repositoryAddress") == "null")
            {
                RepositoryCreation();
            }
        }

        private static void RepositoryCreation()
        {
            UpdateSetting("repositoryAddress", GetStringFromUser("enter repository address"));
        }

        private static bool UserVerification(string login, string password)
        {
           return login == ConfigurationManager.AppSettings.Get("login") && password == ConfigurationManager.AppSettings.Get("password");
        }

        private static string GetStringFromUser(string message)
        {
            {
                Console.WriteLine(message);
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

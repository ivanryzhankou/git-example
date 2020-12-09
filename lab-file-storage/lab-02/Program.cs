﻿using System;
using System.Configuration;
using System.Collections.Specialized;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Collections;


namespace lab_02
{
    class Program
    {
        static void Main(string[] args)
        {
            InterfaceLayer.InterfaceService interfaceService = new InterfaceLayer.InterfaceService();
            DataLayer.BinaryDataRepository dataRepository = new DataLayer.BinaryDataRepository();


            interfaceService.ShowStartMenu();
            //dataRepository.ShowMetaInformation("go.rtf");
        }

        static void RenameFile(string originalName, string newName)
        {
            File.Move(originalName, newName);
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
                Console.WriteLine("Enter your login");
                string login = Console.ReadLine();

                Console.WriteLine("Enter your password");
                string password = Console.ReadLine();

                isUserValid = UserVerification(login, password);

                if (isUserValid == false)
                {
                    Console.Clear();
                    Console.WriteLine("Try again. Wrong login or password.");
                }
            }
            while (isUserValid == false);
            Console.Clear();
        }
    }
}

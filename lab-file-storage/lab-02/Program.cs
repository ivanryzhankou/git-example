using BusinessLayer;
using BusinessLayer.Interfaces;
using DataLayer;
using DataLayer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using PresentationLayer;
using PresentationLayer.Interfaces;
using System;
using System.Configuration;


namespace lab_02
{
    class Program
    {
        private static IServiceProvider _container = GetContainer();
        static void Main(string[] args)
        {
            var _interfaceServise = _container.GetService<IPresentationService>();
            GetUserCredentials();
            CheckForFirstLaunch(_interfaceServise);
            _interfaceServise.ShowStartMenu();
        }

        public static IServiceProvider GetContainer()
        {
            ServiceCollection container = new ServiceCollection();

            container.AddTransient<IPresentationService, PresentationService>();
            container.AddTransient<IBusinessService, BuisnessService>();
            container.AddTransient<IBinaryDataRepository, BinaryDataRepository>();
            container.AddTransient<IConfigurationDataRepository, ConfigurationDataRepository>();
            container.AddTransient<IDataRepository, DataRepository>();

            return container.BuildServiceProvider();
        }

        private static bool ValidateUserCredentials(string login, string password)
        {
            string validLogin = ConfigurationManager.AppSettings.Get("login");
            string validPassword = ConfigurationManager.AppSettings.Get("password");

            return login == validLogin && password == validPassword;
        }

        private static void GetUserCredentials()
        {
            bool isUserValid;

            do
            {
                Console.WriteLine("Enter your login");
                string login = Console.ReadLine();

                Console.WriteLine("Enter your password");
                string password = Console.ReadLine();

                isUserValid = ValidateUserCredentials(login, password);
                if (!isUserValid)
                {
                    Console.Clear();
                    Console.WriteLine("Try again. Wrong login or password.");
                }
            }
            while (isUserValid == false);
            Console.Clear();
        }

        private static void CheckForFirstLaunch(IPresentationService interfaceService)
        {
            if (ConfigurationManager.AppSettings.Get("creationDate") == string.Empty)
            {
                interfaceService.PreparationForFirstLaunch();
            }
        }
    }
}

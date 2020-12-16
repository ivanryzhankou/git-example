using System;
using System.Configuration;


namespace lab_02
{
    class Program
    {
        private static PresentationLayer.PresentationService _interfaceService = new PresentationLayer.PresentationService();

        //public Program()
        //{
        //    _interfaceService = new PresentationLayer.PresentationService();
        //}

        static void Main(string[] args)
        {
            GetUserCredentials();
            CheckForFirstLaunch(_interfaceService);
            _interfaceService.ShowStartMenu();
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

        private static void CheckForFirstLaunch(PresentationLayer.PresentationService interfaceService)
        {
            if (ConfigurationManager.AppSettings.Get("creationDate") == string.Empty)
            {
                interfaceService.PreparationForFirstLaunch();
            }
        }
    }
}

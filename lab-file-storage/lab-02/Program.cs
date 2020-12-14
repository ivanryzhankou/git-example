using System;
using System.Configuration;



namespace lab_02
{
    class Program
    {
        static void Main(string[] args)
        {
            InterfaceLayer.InterfaceService interfaceService = new InterfaceLayer.InterfaceService();

            Logging();
            CheckForFirstLaunch(interfaceService);
            interfaceService.ShowStartMenu();
        }

        //todo: method name must be a verb. 
        private static bool UserVerification(string login, string password)
        {
            return login == ConfigurationManager.AppSettings.Get("login") && password == ConfigurationManager.AppSettings.Get("password");
        }

        //todo: method name must be a verb. 
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
                //todo: try to avoid such syntax. Prefer (!isUserValid)
                if (isUserValid == false)
                {
                    Console.Clear();
                    Console.WriteLine("Try again. Wrong login or password.");
                }
            }
            while (isUserValid == false);
            Console.Clear();
        }

        //todo: interfaceService - strange name, cannot understand for it is for
        private static void CheckForFirstLaunch(InterfaceLayer.InterfaceService interfaceService)
        {
            if (ConfigurationManager.AppSettings.Get("creationDate") == string.Empty)
            {
                interfaceService.PreparationForFirstLaunch();
            }
        }
    }
}

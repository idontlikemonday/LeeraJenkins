using System;
using System.Configuration;

namespace LeeraJenkins.Migrations
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var connection = ConfigurationManager.ConnectionStrings["Tg"].ConnectionString;
                Runner.Initialize(connection);
                Runner.MigrateToLatest();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }
    }
}

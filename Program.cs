using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalIRServerTest
{
    public class Program
    {
        public static string HomeConnectionString = "Data Source=SASOSLAV\\SQLEXPRESS;user=SASOSLAV\\Andrey;Initial Catalog=Education;Integrated Security=true;";
        public static string SchoolConnectionString = "Data Source=169.254.131.3;user=stud;password=Qwerty123456$;Initial Catalog=Education;";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

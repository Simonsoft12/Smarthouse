using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Smarthouse
{
    public class Program
    {
        public enum Enviroment
        {
            Production = 1,
            Simulator = 2,
        }

        public static Enviroment EnviromentType;

        public static void Main(string[] args)
        {

            EnviromentType |= Enviroment.Simulator;
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://0.0.0.0:5500")
                .Build();
    }
}

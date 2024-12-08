using Microsoft.Extensions.DependencyInjection;
using OCRWorker.ProcessLibrary;
using RabbitMQ.Client;
using System;
using System.Diagnostics;

namespace OCRWorker
{
    class Program
    {
        

        static void Main(string[] args)
        {

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IProcessStartInfoFactory, ProcessStartInfoFactory>() // Register ProcessStartupFactory
                .AddSingleton<IProcessFactory, ProcessFactory>() 
                .AddSingleton<IConnectionFactory>(_ =>
                    new ConnectionFactory
                    {
                        HostName = "rabbitmq",
                        UserName = "user",
                        Password = "password"
                    })
                .AddSingleton<OcrWorker>()                                     // Register OcrWorker
                .BuildServiceProvider();

            // Resolve OcrWorker from the DI container
            var worker = serviceProvider.GetService<OcrWorker>();
            worker.Initialize();
            worker.StartAsync();

            Console.WriteLine("OCR Worker is running. Press Ctrl+C to exit.");

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}